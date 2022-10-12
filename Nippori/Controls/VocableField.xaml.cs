﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Nippori.ViewModel;

namespace Nippori.Controls
{
    /// <summary>
    /// Interaction logic for VocableField.xaml
    /// </summary>
    public partial class VocableField : UserControl
    {
        #region .: Private Variables :.

        private VocableFieldViewModel vocableFieldVM;
        private string charUnderCursor = string.Empty;
        private int selStartOffsetPrev = -1;
        private int selEndOffsetPrev = -1;

        #endregion

        #region .: Dependency Properties :.

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VocableField),
                new PropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        #endregion

        #region .: Properties :.

        /// <summary>
        /// Gets or sets the text shown in the vocable field.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region .: Constructors :.

        /// <summary>
        /// Creates a new instance of the <see cref="VocableField"/> class.
        /// </summary>
        public VocableField()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Static constructor of the class.
        /// </summary>
        static VocableField()
        {
            // Override metadata of inherited DependencyProperties to add custom PropertyChangedCallback
            // which passes the updated value to the ViewModel

            FontSizeProperty.OverrideMetadata(typeof(VocableField), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontSizeChanged)));
            BackgroundProperty.OverrideMetadata(typeof(VocableField),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBackgroundChanged)));
            ForegroundProperty.OverrideMetadata(typeof(VocableField),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnForegroundChanged)));
        }

        #endregion

        #region .: Private Methods :.

        private void ResetColorMarking(RichTextBox richTextBox)
        {
            TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            tr.ApplyPropertyValue(ForegroundProperty, Brushes.Black);
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.Text = e.NewValue.ToString();
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.FontSize = (double)e.NewValue;
        }

        private void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.Background = (Brush)e.NewValue;
        }

        private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.Foreground = (Brush)e.NewValue;
        }

        #endregion

        #region .: Event Handlers :.

        #region .: DataContext :.

        private void Border_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is VocableFieldViewModel model)
            {
                vocableFieldVM = model;
            }
        }

        #endregion

        #region .: DependencyProperty :.

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VocableField)d).OnTextChanged(e);
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VocableField)d).OnFontSizeChanged(e);
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VocableField)d).OnBackgroundChanged(e);
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VocableField)d).OnForegroundChanged(e);
        }

        #endregion

        #region .: RichTextBox :.

        private void RichTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;

            TextPointer contentStart = richTextBox.Document.ContentStart;
            TextPointer contentEnd = richTextBox.Document.ContentEnd;

            Point mousePosition = e.GetPosition(richTextBox);
            TextPointer textPointer = richTextBox.GetPositionFromPoint(mousePosition, false);
            if (textPointer != null)
            {
                TextPointer selStart, selEnd;

                // this needs to be done because the selection pointer changes in the middle of
                // the characters, not between them (as one would naturally expect)
                if (textPointer.LogicalDirection == LogicalDirection.Forward)
                {
                    selStart = textPointer.GetPositionAtOffset(0);
                    selEnd = textPointer.GetPositionAtOffset(1);
                }
                else
                {
                    selStart = textPointer.GetPositionAtOffset(-1);
                    selEnd = textPointer.GetPositionAtOffset(0);
                }

                int selStartOffset = contentStart.GetOffsetToPosition(selStart);
                int selEndOffset = contentStart.GetOffsetToPosition(selEnd);
                if ((selStartOffset != selStartOffsetPrev) || (selEndOffset != selEndOffsetPrev))
                {
                    TextRange tr = new TextRange(contentStart, contentEnd);

                    // color whole text to black (to remove previous red marking)
                    tr.ApplyPropertyValue(ForegroundProperty, Brushes.Black);

                    // color the character under mouse cursor to red
                    tr.Select(selStart, selEnd);
                    tr.ApplyPropertyValue(ForegroundProperty, Brushes.Red);

                    charUnderCursor = tr.Text;

                    selStartOffsetPrev = selStartOffset;
                    selEndOffsetPrev = selEndOffset;

                    richTextBox.Cursor = Cursors.Hand;
                }
            }
            else
            {
                ResetColorMarking(richTextBox);
                selStartOffsetPrev = -1;
                selEndOffsetPrev = -1;
                charUnderCursor = string.Empty;
                richTextBox.Cursor = Cursors.Arrow;
            }
        }

        private void RichTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            ResetColorMarking(richTextBox);
        }

        #endregion

        #endregion
    }
}