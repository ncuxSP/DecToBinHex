using System;
using System.ComponentModel;

namespace DecToBinHexTool
{
    public delegate void ComputeNewNumberEvH(int number);

    public interface IView
    {
        event ComputeNewNumberEvH ComputeNewNumber;
        void BindDataSource(BindingList<string> container);
    }
}