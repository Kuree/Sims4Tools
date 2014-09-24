/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies constants defining which symbol to display on a <see cref="CopyableMessageBox"/>.
    /// </summary>
    public enum CopyableMessageBoxIcon
    {
        /// <summary>
        /// The message box contain no symbols.
        /// </summary>
        /// <remarks>This is the default for unknown values.</remarks>
        None = 0,
        /// <summary>
        /// The message box contains a symbol consisting of a lowercase letter i.
        /// This is styled italic and displayed in blue on very pale blue.
        /// </summary>
        Information = 1,
        /// <summary>
        /// The message box contains a symbol consisting of a lowercase letter i.
        /// This is styled italic and displayed in blue on very pale blue.
        /// </summary>
        /// <remarks>This is the same as <see cref="CopyableMessageBoxIcon.Information"/>.</remarks>
        Asterisk = 1,
        /// <summary>
        /// The message box contains a symbol consisting of a question mark.
        /// This is styled regular and displayed in green on very pale green.
        /// </summary>
        Question = 2,
        /// <summary>
        /// The message box contains a symbol consisting of an exclamation mark.
        /// This is styled bold and displayed in black on yellow.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// The message box contains a symbol consisting of an exclamation mark.
        /// This is styled bold and displayed in black on yellow.
        /// </summary>
        /// <remarks>This is the same as <see cref="CopyableMessageBoxIcon.Warning"/>.</remarks>
        Exclamation = 3,
        /// <summary>
        /// The message box contains a symbol consisting of an uppercase letter X.
        /// This is styled bold and displayed in white on red.
        /// </summary>
        Error = 4,
        /// <summary>
        /// The message box contains a symbol consisting of an uppercase letter X.
        /// This is styled bold and displayed in white on red.
        /// </summary>
        /// <remarks>This is the same as <see cref="CopyableMessageBoxIcon.Error"/>.</remarks>
        Hand = 4,
        /// <summary>
        /// The message box contains a symbol consisting of an uppercase letter X.
        /// This is styled bold and displayed in white on red.
        /// </summary>
        /// <remarks>This is the same as <see cref="CopyableMessageBoxIcon.Error"/>.</remarks>
        Stop = 4,
    }

    /// <summary>
    /// Specifies constants defining which buttons to display on a <see cref="CopyableMessageBox"/>.
    /// </summary>
    public enum CopyableMessageBoxButtons
    {
        /// <summary>
        /// The message box contains an OK button.
        /// </summary>
        /// <remarks>This is the default for unknown values.</remarks>
        OK,
        /// <summary>
        /// The message box contains OK and Cancel buttons.
        /// </summary>
        OKCancel,
        /// <summary>
        /// The message box contains Abort, Retry, and Ignore buttons.
        /// </summary>
        AbortRetryIgnore,
        /// <summary>
        /// The message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel,
        /// <summary>
        /// The message box contains Yes and No buttons.
        /// </summary>
        YesNo,
        /// <summary>
        /// The message box contains Retry and Cancel buttons.
        /// </summary>
        RetryCancel,
    }

    /// <summary>
    /// Displays a message box from which the text can be copied.
    /// </summary>
    public static class CopyableMessageBox
    {
        internal static Form OwningForm
        {
            get
            {
                Form owner = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                if (owner != null && (owner.InvokeRequired || owner.IsDisposed || !owner.IsHandleCreated)) owner = null;
                return owner;
            }
        }

        /// <summary>
        /// Displays a message box with the specified text.
        /// The text can be copied.
        /// </summary>
        /// <param name="message">The text to display in the message box.  This text can be copied.</param>
        /// <returns>Always returns <c>0</c>.</returns>
        public static int Show(string message)
        {
            return Show(message, OwningForm != null ? OwningForm.Text : Application.ProductName,
                CopyableMessageBoxIcon.None, new List<string>(new string[] { "OK" }), 0, 0);
        }
        /// <summary>
        /// Displays a message box with the specified text, caption, buttons, icon and default button.
        /// The text can be copied.
        /// </summary>
        /// <param name="message">The text to display in the message box.  This text can be copied.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the <see cref="CopyableMessageBoxButtons"/> values that specifies which buttons to display in the message box.
        /// If not specified, <see cref="CopyableMessageBoxButtons.OK"/> is used.</param>
        /// <param name="icon">One of the <see cref="CopyableMessageBoxIcon"/> values that specifies which icon to display in the message box.
        /// If not specified, <see cref="CopyableMessageBoxIcon.None"/> is used.</param>
        /// <param name="defBtn">The zero-based index of the default button.
        /// If not specified, <c>0</c> is used.</param>
        /// <returns>The zero-based index of the button pressed.</returns>
        public static int Show(string message, string caption,
            CopyableMessageBoxButtons buttons = CopyableMessageBoxButtons.OK,
            CopyableMessageBoxIcon icon = CopyableMessageBoxIcon.None,
            int defBtn = 0)
        {
            int cncBtn = enumToCncBtn(buttons);
            return Show(message, caption, icon, enumToList(buttons), defBtn, cncBtn);
        }
        /// <summary>
        /// Displays a message box with the specified text, caption, icon, buttons, default button and cancel button.
        /// The text can be copied.
        /// </summary>
        /// <param name="message">The text to display in the message box.  This text can be copied.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the <see cref="CopyableMessageBoxButtons"/> values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the <see cref="CopyableMessageBoxIcon"/> values that specifies which icon to display in the message box.</param>
        /// <param name="defBtn">The zero-based index of the default button.</param>
        /// <param name="cncBtn">The zero-based index of the cancel button.</param>
        /// <returns>The zero-based index of the button pressed.</returns>
        public static int Show(string message, string caption, CopyableMessageBoxButtons buttons, CopyableMessageBoxIcon icon, int defBtn, int cncBtn)
        {
            return Show(message, caption, icon, enumToList(buttons), defBtn, cncBtn);
        }

        /// <summary>
        /// Displays a message box with the specified text, caption, icon, buttons, default button and cancel button.
        /// The text can be copied.
        /// </summary>
        /// <param name="message">The text to display in the message box.  This text can be copied.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="icon">One of the <see cref="CopyableMessageBoxIcon"/> values that specifies which icon to display in the message box.</param>
        /// <param name="buttons">A <see cref="IList{T}"/> (where <c>T</c> is <see cref="string"/>) to display as buttons in the message box.</param>
        /// <param name="defBtn">The zero-based index of the default button.</param>
        /// <param name="cncBtn">The zero-based index of the cancel button.</param>
        /// <returns>The zero-based index of the button pressed.</returns>
        public static int Show(string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
        {
            return Show(OwningForm, message, caption, icon, buttons, defBtn, cncBtn);
        }

        /// <summary>
        /// Displays a message box with the specified text, caption, icon, buttons, default button and cancel button.
        /// The text can be copied.
        /// </summary>
        /// <param name="owner">An implementation of <see cref="System.Windows.Forms.IWin32Window"/> that will own the modal dialog box.</param>
        /// <param name="message">The text to display in the message box.  This text can be copied.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="icon">One of the <see cref="CopyableMessageBoxIcon"/> values that specifies which icon to display in the message box.</param>
        /// <param name="buttons">A <see cref="IList{T}"/> (where <c>T</c> is <see cref="string"/>) to display as buttons in the message box.</param>
        /// <param name="defBtn">The zero-based index of the default button.</param>
        /// <param name="cncBtn">The zero-based index of the cancel button.</param>
        /// <returns>The zero-based index of the button pressed.</returns>
        public static int Show(IWin32Window owner, string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
        {
            CopyableMessageBoxInternal cmb = new CopyableMessageBoxInternal(message, caption, icon, buttons, defBtn, cncBtn);

            DialogResult dr;
            if (owner != null) { cmb.Icon = ((Form)owner).Icon; dr = cmb.ShowDialog(owner); } else { dr = cmb.ShowDialog(); }

            if (dr == DialogResult.Cancel) return cncBtn;
            return (cmb.theButton != null) ? buttons.IndexOf(cmb.theButton.Text) : -1;
        }

        private static int enumToCncBtn(CopyableMessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case CopyableMessageBoxButtons.OK: return 0;
                case CopyableMessageBoxButtons.OKCancel: return 1;
                case CopyableMessageBoxButtons.RetryCancel: return 1;
                case CopyableMessageBoxButtons.AbortRetryIgnore: return -1;
                case CopyableMessageBoxButtons.YesNoCancel: return 2;
                case CopyableMessageBoxButtons.YesNo: return -1;
                default: return -1;
            }
        }

        private static IList<string> enumToList(CopyableMessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case CopyableMessageBoxButtons.OKCancel: return new List<string>(new string[] { "&OK", "&Cancel", });
                case CopyableMessageBoxButtons.AbortRetryIgnore: return new List<string>(new string[] { "&Abort", "&Retry", "&Ignore", });
                case CopyableMessageBoxButtons.RetryCancel: return new List<string>(new string[] { "&Retry", "&Cancel", });
                case CopyableMessageBoxButtons.YesNoCancel: return new List<string>(new string[] { "&Yes", "&No", "&Cancel", });
                case CopyableMessageBoxButtons.YesNo: return new List<string>(new string[] { "&Yes", "&No", });
                case CopyableMessageBoxButtons.OK:
                default: return new List<string>(new string[] { "&OK", });
            }
        }

        /// <summary>
        /// Displays a message box containing the specified <see cref="Exception"/>
        /// including a full traceback of inner exceptions.
        /// The text can be copied.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> to display.</param>
        public static void IssueException(Exception ex) { IssueException(ex, "", "Program Exception"); }
        /// <summary>
        /// Displays a message box containing the specified <see cref="Exception"/>
        /// including a full traceback of inner exceptions, with the specified caption.
        /// The text can be copied.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> to display.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static void IssueException(Exception ex, string caption) { IssueException(ex, "", caption); }
        /// <summary>
        /// Displays a message box containing the specified <see cref="Exception"/>
        /// including a full traceback of inner exceptions, with the specified caption.
        /// The <paramref name="prefix"/> text is display before the exception trace.
        /// The text can be copied.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> to display.</param>
        /// <param name="prefix">Text to display before the exception.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static void IssueException(Exception ex, string prefix, string caption)
        {
            System.Text.StringBuilder sb = new Text.StringBuilder();
            sb.Append(prefix);
            sb.Append("\n== START ==");
            for (Exception inex = ex; inex != null; inex = inex.InnerException)
            {
                sb.Append("\nSource: " + inex.Source);
                sb.Append("\nAssembly: " + inex.TargetSite.DeclaringType.Assembly.FullName);
                sb.Append("\n" + inex.Message);
                sb.Append("\n" + inex.StackTrace);
                sb.Append("\n-----");
            }
            sb.Append("\n== END ==");
            CopyableMessageBox.Show(sb.ToString(), caption, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
        }
    }
}
