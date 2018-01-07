using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TutorialBuilder.Tests.Samples.SubFolder
{
    internal abstract class My_class : List<string>, IDisposable
    {
        public void Dispose()
        {
        }
    }

    [Description("foo")]
    [AttributeUsage(AttributeTargets.All)]
    public class MyAttribute : Attribute
    {
        public int Prop { get; set; }
    }

    [Description]
    interface IFoo
    {
        void Bar();
    }
}
