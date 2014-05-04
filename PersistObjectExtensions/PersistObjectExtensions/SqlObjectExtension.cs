using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;


namespace System.PersistObjectExtension
{
    public static class SqlObjectExtension
    {
        public static SqlConnection SqlConnection;

        public static void SqlPersist(this System.Collections.IList __collection, String __tbname, String _pk = "id")
        {
            __collection.SqlPersist(SqlObjectExtension.SqlConnection, __tbname, _pk);
        }
        public static void SqlPersist(this System.Collections.IList __collection, SqlConnection _conn, String __tbname, String _pk = "id")
        {
            foreach (var item in __collection)
            {
                item.SqlPersist(_conn, __tbname, _pk);
            }
        }

        public static void SqlPersist(this object __obj, String __tbname, String _pk = "id")
        {
            SqlPersist(__obj, SqlObjectExtension.SqlConnection, __tbname, _pk);
        }
        public static void SqlPersist(this object __obj, SqlConnection __conn, String __tbname, String _pk = "id")
        {
            // Before checks...
            SqlObjectExtension.TestSqlConnectionState(__conn);

            var ps = from _ps in __obj.GetType().GetProperties()
                     where _ps.Name.ToLower().Trim().Equals(_pk.ToLower().Trim())
                     select _ps;

            if (ps.Count() > 0)
            {
                PropertyInfo p = ps.First();

                Object objectvalue = p.GetValue(__obj);
                if (!(objectvalue is int))
                {
                    throw new Exception("PK is not Int32");
                }
                int iobjvalue = Convert.ToInt32(objectvalue);
                String qy = "";
                if (iobjvalue > 0)
                {
                    qy = getUpdateQuery(__obj, __tbname, _pk);
                }
                else
                {
                    qy = getInsertQuery(__obj, __tbname, _pk);
                }
                SqlCommand cmd = new SqlCommand(qy, __conn);
                int ret = cmd.ExecuteNonQuery();
                if (ret <= 0)
                {
                    throw new Exception("Failed to insert.");
                }

            }
        }


        private static String getUpdateQuery(Object __obj, String __tbname, String _pk = "id")
        {
            String ret = "";
            String updates = "";
            String id = "";

            var _ot = __obj.GetType();
            var _op = _ot.GetProperties();

            foreach (PropertyInfo _pi in _op)
            {
                Object _foval = _pi.GetValue(__obj);
                String _fname = _pi.Name.ToLower().Trim();
                String _fvalue = getSqlValueFromType(_foval);

                if (!_fname.Equals(_pk, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (_fvalue.Length > 0)
                    {

                        if (updates.Length > 0) updates += ",";
                        updates += _fname + "=" + _fvalue;
                    }
                }
                else
                {
                    id = _fvalue;
                }
            }

            ret = "update {tbname} set {update} where {pk}={pkv}";
            ret = ret.Replace("{tbname}", __tbname);
            ret = ret.Replace("{update}", updates);
            ret = ret.Replace("{pk}", _pk);
            ret = ret.Replace("{pkv}", id);


            return ret;
        }

        private static String getInsertQuery(Object __obj, String __tbname, String _pk = "id")
        {
            String ret = "";
            String fields = "";
            String values = "";

            var _ot = __obj.GetType();
            var _op = _ot.GetProperties();

            foreach (PropertyInfo _pi in _op)
            {
                Object _foval = _pi.GetValue(__obj);
                String _fname = _pi.Name.ToLower().Trim();
                String _fvalue = getSqlValueFromType(_foval);

                if (!_fname.Equals(_pk, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (_fvalue.Length > 0)
                    {

                        if (fields.Length > 0) fields += ",";
                        fields += _fname;

                        if (values.Length > 0) values += ",";
                        values += _fvalue;
                    }
                }
            }

            fields = "(" + fields + ")";
            values = "(" + values + ")";

            ret = "insert into {tbname} {fields} values {values}  SELECT SCOPE_IDENTITY() ";
            ret = ret.Replace("{tbname}", __tbname);
            ret = ret.Replace("{fields}", fields);
            ret = ret.Replace("{values}", values);

            return ret;
        }

        private static String getSqlValueFromType(Object __obj)
        {
            String ret = "";

            if (__obj == DBNull.Value) return "";

            if (__obj is int)
            {
                ret = ((int)__obj).ToString();
            }
            else if (__obj is String)
            {
                ret = "'" + (String)__obj + "'";
            }
            else if (__obj is DateTime)
            {
                DateTime dtobj = (DateTime)__obj;
                ret = "'" + (dtobj).ToString("yyyyMMdd HH:mm:ss") + "'";
                if (dtobj == DateTime.MinValue) ret = "";
            }
            else if (__obj is long)
            {
                ret = ((long)__obj).ToString();
            }
            else if (__obj is double)
            {
                ret = ((double)__obj).ToString();
            }
            else
            {
                ret = "'" + __obj.ToString() + "'";
            }

            return ret;
        }

        private static void TestSqlConnectionState(SqlConnection __conn)
        {
            if (__conn == null) throw new Exception("SqlConnection is NULL");

            int __tryTestConnectionStateCount = 0;
            int __tryTestConnectionState = 0;
            bool __ConnectionStateTestedOK = false;
            do
            {
                switch (__conn.State)
                {
                    case System.Data.ConnectionState.Broken:
                        break;
                    case System.Data.ConnectionState.Closed:
                        __conn.Open();
                        __tryTestConnectionState++;
                        break;
                    case System.Data.ConnectionState.Connecting:
                        System.Threading.Thread.Sleep(1000);
                        if (__tryTestConnectionStateCount < 3) __tryTestConnectionState++;
                        break;
                    case System.Data.ConnectionState.Executing:
                        System.Threading.Thread.Sleep(1000);
                        if (__tryTestConnectionStateCount < 3) __tryTestConnectionState++;
                        break;
                    case System.Data.ConnectionState.Fetching:
                        System.Threading.Thread.Sleep(1000);
                        if (__tryTestConnectionStateCount < 3) __tryTestConnectionState++;
                        break;
                    case System.Data.ConnectionState.Open:
                        __ConnectionStateTestedOK = true;
                        break;
                    default:
                        break;
                }
            } while (__tryTestConnectionState > 0);

            if (!__ConnectionStateTestedOK) throw new Exception("SqlConnectionState is " + __conn.State.ToString());

        }

        public static void LoadFromQuery(this Object __obj, String __query)
        {
            LoadFromQuery(__obj, __query, SqlObjectExtension.SqlConnection);
        }
        public static void LoadFromQuery(this Object __ObjectToSet, String __query, SqlConnection __conn)
        {
            SqlObjectExtension.TestSqlConnectionState(__conn);

            SqlCommand __SqlCommand = new SqlCommand(__query, __conn);

            SqlDataReader __SqlDataReader = __SqlCommand.ExecuteReader();

            var __ObjectToSet_Type = __ObjectToSet.GetType();
            var __ObjectToSet_Properties = __ObjectToSet_Type.GetProperties();
            var __IsIList = __ObjectToSet is System.Collections.IList;
            System.Collections.IList __ListToSet = null;
            Object __ObjectInAction = __ObjectToSet;
            if (__IsIList)
            {
                var __ObjectToSet_Item_Property = __ObjectToSet_Type.GetProperty("Item");
                __ObjectToSet_Type = __ObjectToSet_Item_Property.PropertyType;
                __ObjectToSet_Properties = __ObjectToSet_Item_Property.PropertyType.GetProperties();

                __ListToSet = (System.Collections.IList)__ObjectToSet;
                __ListToSet.Clear();
            }

            while (__SqlDataReader.Read())
            {
                if (__IsIList)
                {
                    __ObjectInAction = Activator.CreateInstance(__ObjectToSet_Type);
                    __ListToSet.Add(__ObjectInAction);
                }

                for (int __counter = 0; __counter < __SqlDataReader.FieldCount; __counter++)
                {
                    var __ColumnName = __SqlDataReader.GetName(__counter);
                    var __ColumnType = __SqlDataReader.GetFieldType(__counter);
                    var __ColumnValue = __SqlDataReader.GetValue(__counter);

                    var __QueriedObjectToSetProperties = from _p in __ObjectToSet_Properties
                                                         where _p.Name.ToLower().Trim().Equals(__ColumnName.ToLower().Trim())
                                                         select _p;

                    if (__QueriedObjectToSetProperties.Count() > 0)
                    {
                        var __QueriedObjectToSetProperty = __QueriedObjectToSetProperties.First();
                        bool __IsExpectedType = __QueriedObjectToSetProperty.PropertyType == __ColumnType;

                        if (__IsExpectedType && __ColumnValue != DBNull.Value)
                        {
                            __QueriedObjectToSetProperty.SetValue(__ObjectInAction, __ColumnValue);
                        }
                    }

                }
            }

            __SqlDataReader.Close();
        }




    }
}
