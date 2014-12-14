using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Linq2Datatable
{
    /// <summary>
    /// 给DataTable增加诸如Where、ForEach、All、Count、Sum等Linq操作
    /// </summary>
    public static class LinqToDataTable
    {

        /// <summary>
        /// 从DataTable中获取某个字段符合给定条件的记录组成的DataTable
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="name">字段名称</param>
        /// <param name="predicate">Func委托</param>
        /// <returns>DataTable</returns>
        public static DataTable Where<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector.");
            if (table.Columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            DataTable _table = new DataTable(table.TableName);
            for (int i = 0; i < table.Columns.Count; i++) {
                _table.Columns.Add(table.Columns[i].ColumnName, table.Columns[i].DataType);
            }
            for (int i = 0; i < table.Rows.Count; i++) {
                if (predicate((T)table.Rows[i][name])) {
                    DataRow newRow = _table.NewRow();
                    for (int index = 0; index < table.Columns.Count; index++) {
                        newRow[table.Columns[index].ColumnName] = table.Rows[i][table.Columns[index]];
                    }
                    _table.Rows.Add(newRow);
                }
            }
            return _table;
        }
        /// <summary>
        /// 从DataTable中获取某个字段组成的集合
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="selector">Func委托</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> Select<T>(this DataTable table, string name)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid field name");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            IList<T> pList = new List<T>(table.Rows.Count);
            for (int i = 0; i < table.Rows.Count; i++) {
                pList.Add((T)table.Rows[i][name]);
            }
            return pList;
        }
        /// <summary>
        /// 获取DataTable中某个字段符合给定条件的处理过的集合
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="name">字段名称</param>
        /// <param name="selector">Func委托</param>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(this DataTable table, string name, Func<T, T> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid field name");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            IList<T> pList = new List<T>();
            for (int i = 0; i < table.Rows.Count; i++) {
                pList.Add(selector((T)table.Rows[i][name]));
            }
            return pList;
        }

        /// <summary>
        /// 获取DataTable中某个字段符合给定条件的集合
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="name">字段名称</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid field name");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            IList<T> pList = new List<T>();
            for (int i = 0; i < table.Rows.Count; i++) {
                if (predicate((T)table.Rows[0][name])) {
                    pList.Add((T)table.Rows[i][name]);
                }
            }
            return pList;
        }
        /// <summary>
        /// 计算table中某个字段的合计
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="selector">Func委托</param>
        /// <returns>int</returns>
        public static int Sum(this DataTable table, Func<int, string> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            string name = selector(0);
            if (!columns.Contains(name)) throw new ArgumentNullException("invalid field name");
            if (columns[name].DataType != typeof(int)) throw new ArgumentException("int");
            return table.Select<int>(name).Sum();
        }
        /// <summary>
        /// 计算DataTable中某个字段的合计
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="selector">Func委托</param>
        /// <returns>double</returns>
        public static double Sum(this DataTable table, Func<double, string> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            string name = selector(0.0);
            if (!columns.Contains(name)) throw new ArgumentNullException("invalid field name");
            if (columns[name].DataType != typeof(double)) throw new ArgumentException("double");
            return table.Select<double>(name).Sum();
        }
        /// <summary>
        /// 计算DataTable中某个字段的所有记录合计
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="selector">Func委托</param>
        /// <returns>decimal</returns>
        public static decimal Sum(this DataTable table, Func<decimal, string> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            DataColumnCollection columns = table.Columns;
            string name = selector(decimal.One);
            if (!columns.Contains(name)) throw new ArgumentNullException("invalid field name");
            if (columns[name].DataType != typeof(decimal)) throw new ArgumentException("decimal");
            return table.Select<decimal>(name).Sum();
        }
        /// <summary>
        /// 判断给定DataTable中是否存在某个字段值符合给定条件的记录
        /// </summary>
        /// <typeparam name="T">给定字段的数据类型</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="name">字段名称</param>
        /// <param name="predicate">Func委托</param>
        /// <returns>True/False</returns>
        public static bool Contains<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            foreach (DataRow row in table.Rows) {
                if (predicate((T)row[name]))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取DataTable中符合某个条件的唯一记录
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static DataRow Single<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            DataRow result = null;
            long count = 0;
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                if (predicate((T)current[name])) {
                    count++;
                    result = current;
                }
            }
            switch (count) {
                case 0: throw new Exception("No Match");
                case 1: return result;
            }
            throw new Exception("Two or More Match");
        }
        /// <summary>
        /// 获取DataTable中符合某个条件的唯一记录；记录不存在，返回null
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static DataRow SingleOrDefault<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            DataRow result = null;
            long count = 0;
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                if (predicate((T)current[name])) {
                    count++;
                    result = current;
                }
            }
            switch (count) {
                case 0: return null;
                case 1: return result;
            }
            throw new Exception("Two or More Match");
        }

        /// <summary>
        /// Datatable中记录的数目
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static int Count(this DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            return table.Rows.Count;
        }

        /// <summary>
        /// 计算DataTable中某个给定字段符合特定值的记录个数
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static int Count<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            int count = 0;
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                if (predicate((T)current[name])) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 给DataTable中的每条记录执行某个操作
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="func">Func委托</param>
        public static void ForEach(this DataTable table, Action<DataRow> action)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (action == null) throw new ArgumentNullException("action");
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                action(current);
            }
        }
        /// <summary>
        /// 判断DataTable中的所有记录某个字段的值全都符合某个条件
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static bool All<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            bool flag = true;
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                if (!predicate((T)current[name])) {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 判断DataTable中的所有记录某个字段的值是否至少有一个符合条件
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="predicate">Func委托</param>
        /// <returns></returns>
        public static bool Any<T>(this DataTable table, string name, Func<T, bool> predicate)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (predicate == null) throw new ArgumentNullException("predicate");
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            bool flag = false;
            for (int i = 0; i < table.Rows.Count; i++) {
                DataRow current = table.Rows[i];
                if (predicate((T)current[name])) {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 某个字段的最大值
        /// </summary>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="table"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T Max<T>(this DataTable table, Func<string> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (selector == null) throw new ArgumentNullException("selector");
            string name = selector();
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            Comparer<T> comparer = Comparer<T>.Default;
            T value = default(T);
            for (int i = 0; i < table.Rows.Count; i++) {
                if (comparer.Compare(value, (T)table.Rows[i][name]) < 0) {
                    value = (T)table.Rows[i][name];
                }
            }
            return value;
        }
        /// <summary>
        /// DataTable中某个字段的最小值
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="selector">Func委托</param>
        /// <returns>T</returns>
        public static T Min<T>(this DataTable table, Func<string> selector)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (selector == null) throw new ArgumentNullException("selector");
            string name = selector();
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            Comparer<T> comparer = Comparer<T>.Default;
            T value = default(T);
            for (int i = 0; i < table.Rows.Count; i++) {
                if (comparer.Compare(value, (T)table.Rows[i][name]) > 0) {
                    value = (T)table.Rows[i][name];
                }
            }
            return value;
        }
        /// <summary>
        /// DataTable中某个字段具有最大值的记录
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="collection">DataRowCollection</param>
        /// <param name="selector">Func委托</param>
        /// <returns></returns>
        public static DataRow Max<T>(this DataRowCollection collection, Func<string> selector)
        {
            if (collection == null) throw new ArgumentNullException("table");
            if (selector == null) throw new ArgumentNullException("selector");
            string name = selector();
            DataTable table = collection[0].Table;
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            Comparer<T> comparer = Comparer<T>.Default;
            DataRow result = null;
            T value = default(T);
            foreach (DataRow _row in collection) {
                if (comparer.Compare(value, (T)_row[name]) < 0) {
                    value = (T)_row[name];
                    result = _row;
                }
            }
            return result;
        }
        /// <summary>
        /// DataTable中某个字段具有最小值的记录
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="collection">DataRowCollection</param>
        /// <param name="selector">Func委托</param>
        /// <returns></returns>
        public static DataRow Min<T>(this DataRowCollection collection, Func<string> selector)
        {
            if (collection == null) throw new ArgumentNullException("table");
            if (selector == null) throw new ArgumentNullException("selector");
            string name = selector();
            DataTable table = collection[0].Table;
            DataColumnCollection columns = table.Columns;
            if (!columns.Contains(name)) throw new ArgumentException("invalid selector");
            if (columns[name].DataType != typeof(T)) throw new ArgumentException("T");
            Comparer<T> comparer = Comparer<T>.Default;
            DataRow result = null;
            T value = default(T);
            foreach (DataRow _row in collection) {
                if (comparer.Compare(value, (T)_row[name]) > 0) {
                    value = (T)_row[name];
                    result = _row;
                }
            }
            return result;
        }
    }
}
