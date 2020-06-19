using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ProSum.Models.DataBase
{
    public class DataRowParser
    {
        private DataRow row;
        
        public DataRowParser(DataRow row)
        {
            this.row = row;
        }

        public T GetField<T>(string columnName)
        {
            return row.Field<T>(columnName);
        }

        public Dictionary<string, object> GetAllFields()
        {
            Dictionary<string, object> rowValues = new Dictionary<string, object>();
            
            foreach(DataColumn column in row.Table.Columns)
            {
                rowValues.Add(column.ColumnName, row.Field<object>(column));
            }

            return rowValues;
        }

        public T ParseObject<T>()
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertyInfos = result.GetType().GetProperties();
            foreach(PropertyInfo prop in propertyInfos)
            {
                if (row.Table.Columns.Contains(prop.Name))
                {
                    foreach(DataColumn column in row.Table.Columns)
                    {
                        if(column.ColumnName == prop.Name && row.ItemArray[column.Ordinal] != null && row.ItemArray[column.Ordinal].GetType() != typeof(DBNull))
                        {
                            switch (prop.PropertyType.Name)
                            {
                                case "Guid":
                                    prop.SetValue(result, Guid.Parse(row.ItemArray[column.Ordinal].ToString()));
                                    break;

                                case "DepartmentEnum":
                                    prop.SetValue(result, Enum.Parse(typeof(DepartmentEnum), row.ItemArray[column.Ordinal].ToString()));
                                    break;

                                case "LogEntryUpdateType":
                                    prop.SetValue(result, Enum.Parse(typeof(LogEntryUpdateType), row.ItemArray[column.Ordinal].ToString()));
                                    break;

                                case "Status":
                                    prop.SetValue(result, Enum.Parse(typeof(Step.Status), row.ItemArray[column.Ordinal].ToString()));
                                    break;

                                case "RolesEnum":
                                    prop.SetValue(result, Enum.Parse(typeof(RolesEnum), row.ItemArray[column.Ordinal].ToString()));
                                    break;

                                default:
                                    prop.SetValue(result, Convert.ChangeType(row.ItemArray[column.Ordinal], prop.PropertyType));
                                    break;
                                
                            }
                        }
                    }
                    
                }
            }
            return result;
        }
    }
}
