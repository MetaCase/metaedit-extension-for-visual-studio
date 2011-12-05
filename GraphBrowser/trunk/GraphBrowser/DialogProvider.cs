using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MetaCase.GraphBrowser
{
    class DialogProvider
    {
        ///<summary>
        ///Simple information message dialog with OK button.
        /// </summary>
        /// <param name="message">the message in the window.</param>
        /// <param name="title">the title for the dialog window</param>
	    public static void ShowMessageDialog(string message, string title){
            MessageBox.Show(message, title);
	    }
	
	    ///<summary>
        ///Message dialog to for asking questions from user. 
        /// </summary>
        /// <param name="message">the message in the window</param>
        /// <param name="title">the title for the dialog window</param>
        /// <returns>true if OK clicked else false</returns>
	    public static Boolean ShowYesNoMessageDialog(string message, string title){
            switch (MessageBox.Show(message,title, MessageBoxButtons.YesNo)) 
            {
                case DialogResult.Yes:
                    return true;

                case DialogResult.No:
                    return false;
            }
            return false;
	    }
    }
}
