using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace build
{
    static class TestHarness
    {
        [STAThread]
        static int Main(params string[] args)
        {
            System.Windows.Forms.CopyableMessageBox.Show(
                "Hello world\n\n\n\nAnd some more text that goes along a bit" +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                "And some more text that goes along a bit " +
                ""
                , "Caption", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
            System.Windows.Forms.CopyableMessageBox.Show("Hello world");
            System.Windows.Forms.CopyableMessageBox.Show("Hello world\n\n\n\nAnd some more text that goes along a bit");
            System.Windows.Forms.CopyableMessageBox.Show("Hello world", "Caption", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
            System.Windows.Forms.CopyableMessageBox.Show("Hello world\n\n\n\nAnd some more text that goes along a bit", "Caption", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
            Console.ReadKey();
            return 0;
        }
    }
}
