using System;
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
        private int selStartOffsetPrev = -1;
        private int selEndOffsetPrev = -1;

        #endregion

        #region .: Dependency Properties :.

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VocableField),
                new PropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        public static readonly DependencyProperty KanjiFeaturesEnabledProperty =
            DependencyProperty.Register("KanjiFeaturesEnabled", typeof(bool), typeof(VocableField),
                new PropertyMetadata(true, new PropertyChangedCallback(OnKanjiFeaturesEnabledChanged)));

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

        /// <summary>
        /// Gets or sets boolean value which determines if the kanji features (character highlighting, action buttons) are enabled.
        /// </summary>
        public bool KanjiFeaturesEnabled
        {
            get => (bool)GetValue(KanjiFeaturesEnabledProperty);
            set => SetValue(KanjiFeaturesEnabledProperty, value);
        }

        /// <summary>
        /// Gets the character under mouse cursor.
        /// </summary>
        public string CharacterUnderCursor { get; private set; } = string.Empty;

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

        #region .: RichTextBox Character Highlighting :.

        private void UpdateCharacterHighlighting(RichTextBox richTextBox, Point mousePosition)
        {
            TextPointer contentStart = richTextBox.Document.ContentStart;
            TextPointer contentEnd = richTextBox.Document.ContentEnd;

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

                    CharacterUnderCursor = tr.Text;

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
                CharacterUnderCursor = string.Empty;
                richTextBox.Cursor = Cursors.Arrow;
            }
        }

        #endregion

        private void ResetColorMarking(RichTextBox richTextBox)
        {
            TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            tr.ApplyPropertyValue(ForegroundProperty, Brushes.Black);
        }

        #region .: PropertyChanged Callbacks :.

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.Text = e.NewValue.ToString();
        }

        private void OnKanjiFeaturesEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.KanjiFeaturesEnabled = (bool)e.NewValue;
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

        private static void OnKanjiFeaturesEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VocableField)d).OnKanjiFeaturesEnabledChanged(e);
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
            if (vocableFieldVM.KanjiFeaturesEnabled)
            {
                RichTextBox richTextBox = (RichTextBox)sender;
                Point mousePosition = e.GetPosition(richTextBox);

                UpdateCharacterHighlighting(richTextBox, mousePosition);
            }
        }

        private void RichTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (vocableFieldVM.KanjiFeaturesEnabled)
            {
                RichTextBox richTextBox = (RichTextBox)sender;
                ResetColorMarking(richTextBox);
            }
        }

        private void RichTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (vocableFieldVM.KanjiFeaturesEnabled && (CharacterUnderCursor != string.Empty))
            {
                CharacterMouseDown?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #region .: WrapPanel :.

        private void WrapPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            WrapPanel wrapPanel = (WrapPanel)sender;

            foreach (UIElement uiElement in wrapPanel.Children)
            {
                uiElement.Visibility = Visibility.Visible;
            }
        }

        private void WrapPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            WrapPanel wrapPanel = (WrapPanel)sender;

            foreach (UIElement uiElement in wrapPanel.Children)
            {
                uiElement.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region .: Button :.

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag is ButtonCommands cmd)
            {
                ButtonClick?.Invoke(this, new ButtonClickEventArgs(cmd));
            }
        }

        #endregion

        #endregion

        #region .: Events :.

        /// <summary>
        /// Occurs when the right mouse button is pressed while the cursor points on certain character.
        /// </summary>
        public event EventHandler CharacterMouseDown;
        /// <summary>
        /// Occurs when one of the command buttons is clicked.
        /// </summary>
        public event EventHandler<ButtonClickEventArgs> ButtonClick;

        public event EventHandler DebugEvent;

        #endregion

    }
}
