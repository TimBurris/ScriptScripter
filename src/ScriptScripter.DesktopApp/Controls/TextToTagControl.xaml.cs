using System;
using System.Collections.Generic;
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

namespace ScriptScripter.DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for TextToTagControl.xaml
    /// </summary>
    public partial class TextToTagControl : UserControl
    {
        public TextToTagControl()
        {
            InitializeComponent();
            richTextBox.TextChanged += richTextBox_TextChanged;
        }

        private static void OnTagListSourceChanged(DependencyObject source,
          DependencyPropertyChangedEventArgs e)
        {
            TextToTagControl control = source as TextToTagControl;
            var tags = control?.TagListSource?.ToList();

            if (tags != null)
                control.SetTags(tags);
        }

        protected void SetTags(IList<string> tags)
        {
            richTextBox.TextChanged -= richTextBox_TextChanged;

            var para = richTextBox.CaretPosition.Paragraph;
            foreach (var t in tags)
            {
                var container = CreateTagContainer(t, addToListSource: false);
                para.Inlines.Add(container);
            }

            richTextBox.TextChanged += richTextBox_TextChanged;

        }

        public IList<string> TagListSource
        {
            get { return (IList<string>)GetValue(TagListSourceProperty); }
            set { SetValue(TagListSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagListSourceProperty =
            DependencyProperty.Register(
                        "TagListSource",
                        typeof(IList<string>),
                        typeof(TextToTagControl),
                        new PropertyMetadata(defaultValue: null,
                            propertyChangedCallback: new PropertyChangedCallback(OnTagListSourceChanged)
                            )
                        );

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = richTextBox.CaretPosition.GetTextInRun(LogicalDirection.Backward);
            var tagName = this.GetTagName(text);
            if (tagName != null)
            {
                ReplaceTextWithTag(text, tagName);
            }
        }

        private string GetTagName(string text)
        {
            if (text.EndsWith(";"))
            {
                // Remove the ';'
                return text.Substring(0, text.Length - 1).Trim();
            }

            return null;
        }

        private void ReplaceTextWithTag(string inputText, string tagName)
        {
            // Remove the handler temporarily as we will be modifying tags below, causing more TextChanged events
            richTextBox.TextChanged -= richTextBox_TextChanged;

            var para = richTextBox.CaretPosition.Paragraph;

            var matchedRun = para.Inlines.FirstOrDefault(inline =>
            {
                var run = inline as Run;
                return (run != null && run.Text.EndsWith(inputText));
            }) as Run;
            if (matchedRun != null) // Found a Run that matched the inputText
            {
                var container = CreateTagContainer(tagName, addToListSource: true);
                para.Inlines.InsertBefore(matchedRun, container);

                // Remove only if the Text in the Run is the same as inputText, else split up
                if (matchedRun.Text == inputText)
                {
                    para.Inlines.Remove(matchedRun);
                }
                else // Split up
                {
                    var index = matchedRun.Text.IndexOf(inputText) + inputText.Length;
                    var tailEnd = new Run(matchedRun.Text.Substring(index));
                    para.Inlines.InsertAfter(matchedRun, tailEnd);
                    para.Inlines.Remove(matchedRun);
                }
            }

            richTextBox.TextChanged += richTextBox_TextChanged;
        }

        private InlineUIContainer CreateTagContainer(string tagName, bool addToListSource)
        {
            var presenter = new TagControl()
            {
                Text = tagName
            };
            presenter.Unloaded += tagControl_Unloaded;
            if (addToListSource)
                this.TagListSource?.Add(tagName);
            _tagControls.Add(presenter);

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }

        private void tagControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var tc = sender as TagControl;
            if (tc == null)
                return;

            this.TagListSource?.Remove(tc.Text);
            tc.Unloaded -= tagControl_Unloaded;

            System.Diagnostics.Debug.WriteLine($"removed Tag control with value{tc.Text} and datacontext is {this.DataContext}");
            _tagControls.Remove(tc);
        }

        private List<TagControl> _tagControls = new List<TagControl>();
    }
}
