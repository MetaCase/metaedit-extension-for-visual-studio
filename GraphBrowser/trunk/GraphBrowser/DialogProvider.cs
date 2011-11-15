using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MetaCase.GraphBrowser
{
    class DialogProvider
    {
        /**
	     * Simple information message dialog with OK button.
	     * @param message the message in the window.
	     * @param title the title for the dialog window.
	     */
	    public static void ShowMessageDialog(String message, String title){
            MessageBox.Show(message, title);
	    }
	
	    /**
	     * Message dialog to for asking questions from user. 
	     * @param message the message in the window.
	     * @param title the title for the dialog window.
	     * @return true if <b>OK</b> clicked, false if <b>Cancel</b> cliked.
	     */
	    public static Boolean ShowYesNoMessageDialog(String message, String title){
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
