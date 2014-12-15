using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Linq2Datatable
{
    public static class QuickSort
    {
        public static void Sort<T>(DataTable table,string name, int left, int right,bool descending=false) where T : IComparable<T>
        {
            if (left < right) {
                int partion = 0;
                if (descending) {
                    partion = Partion2<T>(table, name, left, right);
                } else
                    Partion<T>(table, name, left, right);
                Sort<T>(table,name, left, partion - 1);
                Sort<T>(table, name, partion + 1, right);
            }
        }

        private static int Partion2<T>(DataTable table, string name, int left, int right) where T : IComparable<T>
        {
            int i = left, j = right + 1;
            T value = (T)table.Rows[left][name];
            while (true) {
                while (((T)table.Rows[++i][name]).CompareTo(value) > 0) {
                    if (i == right)
                        break;
                }
                while (((T)table.Rows[--j][name]).CompareTo(value) < 0) {
                    if (j == left)
                        break;
                }
                if (i >= j)
                    break;
                Swap(table, i, j);
            }
            Swap(table, j, left);
            return j;
        }

        private static int Partion<T>(DataTable table,string name, int left, int right) where T : IComparable<T>
        {
            int i = left, j = right+1;
            T value = (T)table.Rows[left][name];
            while (true) {
                while (((T)table.Rows[++i][name]).CompareTo(value) < 0) {
                    if (i == right)
                        break;
                }
                while (((T)table.Rows[--j][name]).CompareTo(value) > 0) {
                    if (j == left)
                        break;
                }
                if (i >= j)
                    break;
                Swap(table,i,j);
            }
            Swap(table,j,left);
            return j;
        }

        private static void Swap(DataTable table, int i,int j)
        {
            DataRow newRow = table.NewRow();
            for (int index = 0; index < table.Columns.Count; index++) {
                newRow[table.Columns[index]] = table.Rows[i][table.Columns[index]];
            }
            for (int index = 0; index < table.Columns.Count; index++) {
                table.Rows[i][table.Columns[index]] = table.Rows[j][table.Columns[index]];
            }
            for (int index = 0; index < table.Columns.Count; index++) {
                table.Rows[j][table.Columns[index]] = newRow[table.Columns[index]];
            }
        }
    }
}
