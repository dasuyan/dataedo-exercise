namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private IEnumerable<ImportedObject> _importedObjects;

        public void ImportAndPrintData(string fileToImport)
        {
            _importedObjects = new List<ImportedObject>();

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (line != "")
                {
                    importedLines.Add(line);
                }
            }
            

            foreach (var importedLine in importedLines)
            {
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();
                var valuesLength = values.Length;
                
                if (valuesLength == 7)
                {
                    importedObject.Type = values[0];
                    importedObject.Name = values[1];
                    importedObject.Schema = values[2];
                    importedObject.ParentName = values[3];
                    importedObject.ParentType = values[4];
                    importedObject.DataType = values[5];
                    importedObject.IsNullable = values[6];
                    
                    ((List<ImportedObject>)_importedObjects).Add(importedObject);
                }
                else
                {
                    Console.WriteLine("Invalid line: " + importedLine);
                }
            }

            // clear and correct imported data
            foreach (var importedObject in _importedObjects)
            {
                importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            }

            // assign number of children
            for (var i = 0; i < _importedObjects.Count(); i++)
            {
                var importedObject = _importedObjects.ToArray()[i];
                foreach (var impObj in _importedObjects)
                {
                    if (impObj.ParentType != importedObject.Type) continue;
                    if (impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                    }
                }
            }

            foreach (var database in _importedObjects)
            {
                if (database.Type != "DATABASE") continue;
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                // print all database's tables
                foreach (var table in _importedObjects)
                {
                    if (table.ParentType.ToUpper() != database.Type) continue;
                    if (table.ParentName != database.Name) continue;
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    // print all table's columns
                    foreach (var column in _importedObjects)
                    {
                        if (column.ParentType.ToUpper() != table.Type) continue;
                        if (column.ParentName == table.Name)
                        {
                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                        }
                    }
                }
            }
        }
    }
}
