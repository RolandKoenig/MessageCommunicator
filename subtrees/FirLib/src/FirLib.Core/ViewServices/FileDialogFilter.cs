using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ObjectPooling;

namespace FirLib.Core.ViewServices;

public class FileDialogFilter
{
    public string Name { get; set; }

    public List<string> Extensions { get; } = new();

    public FileDialogFilter(string name, params string[] extensions)
    {
        this.Name = name;
        this.Extensions.AddRange(extensions);
    }

    public static string BuildFilterString(IEnumerable<FileDialogFilter> filters)
    {
        var strBuilder = PooledStringBuilders.Current.TakeStringBuilder();
        try
        {
            var isFirstFilter = true;
            foreach(var actFilter in filters)
            {
                if (!isFirstFilter)
                {
                    strBuilder.Append('|');
                }
                isFirstFilter = false;

                actFilter.BuildFilterString(strBuilder);
            }

            return strBuilder.ToString();
        }
        finally
        {
            PooledStringBuilders.Current.ReRegisterStringBuilder(strBuilder);
        }
    }

    public string BuildFilterString()
    {
        var strBuilder = PooledStringBuilders.Current.TakeStringBuilder();
        try
        {
            this.BuildFilterString(strBuilder);

            return strBuilder.ToString();
        }
        finally
        {
            PooledStringBuilders.Current.ReRegisterStringBuilder(strBuilder);
        }
    }

    public void BuildFilterString(StringBuilder strBuilder)
    {
        strBuilder.Append(this.Name);
        strBuilder.Append(' ');
        strBuilder.Append('(');
        for (var loop = 0; loop < this.Extensions.Count; loop++)
        {
            if (loop > 0)
            {
                strBuilder.Append(',');
                strBuilder.Append(' ');
            }

            strBuilder.Append('*');
            strBuilder.Append('.');
            strBuilder.Append(this.Extensions[loop]);
        }
        strBuilder.Append(')');

        strBuilder.Append('|');
        for (var loop = 0; loop < this.Extensions.Count; loop++)
        {
            if (loop > 0)
            {
                strBuilder.Append(',');
                strBuilder.Append(' ');
            }

            strBuilder.Append('*');
            strBuilder.Append('.');
            strBuilder.Append(this.Extensions[loop]);
        }
    }
}