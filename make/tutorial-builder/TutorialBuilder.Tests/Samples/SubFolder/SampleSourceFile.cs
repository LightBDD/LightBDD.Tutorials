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

    public class MySmallerClass
    {
        public string CallMeNow()
        {
            throw new NotImplementedException();
        }
    }

    public class MyBiggerClass
    {
        private string _value;

        public MyBiggerClass()
        {
            _value = CallMeNow();
            _value = CallMeWithDefault();
            _value = CallMeWithGeneric(4);
        }

        public string CallMeWithDefault(int input = 0)
        {
            throw new NotImplementedException();
        }

        public string CallMeWithGeneric<T>(T input) where T : new()
        {
            throw new NotImplementedException();
        }

        [Description("foo")]
        public string CallMeNow()
        {
            throw new NotImplementedException();
        }
    }
}
