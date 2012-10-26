using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ArmText.Typing
{
  public class KeySelectedEventArgs : EventArgs
  {
    public Key SelectedKey { get; set; }
  }
}
