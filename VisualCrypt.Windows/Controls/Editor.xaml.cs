﻿using Windows.UI.Xaml.Controls;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Windows.Controls.EditorSupport;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class Editor : IEditor
    {
        readonly EditorViewModel _viewModel = new EditorViewModel();
        public Editor()
        {
            InitializeComponent();
            Loaded += (s, e) => _viewModel.OnViewInitialized(this,Context);
        }

        public IEditorContext Context { private get; set; }

        #region IEditor

        public TextBox TextBox1
        {
            get { return _textBox1; }
        }

        public TextBox TextBoxFind
        {
            get { return null; }
        }

        public TextBox TextBoxFindReplace
        {
            get { return null; }
        }

        public TextBox TextBoxGoTo
        {
            get { return null; }
        }

        public UserControl EditorControl
        {
            get { return this; }
        }

       

        #endregion
    }
}
