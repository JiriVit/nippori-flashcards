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
            // Override metadata of inherited DependencyProperty FontSize to add custom PropertyChangedCallback
            // which passes the updated value to the ViewModel
            FontSizeProperty.OverrideMetadata(typeof(VocableField), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontSizeChanged)));
        }

        #endregion

        #region .: Private Methods :.

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.Text = e.NewValue.ToString();
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            vocableFieldVM.FontSize = (double)e.NewValue;
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

        #endregion

        #endregion
    }
}
