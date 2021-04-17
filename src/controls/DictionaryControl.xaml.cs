using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Enable_Now_Konnektor_Editor.src.helper;

namespace Enable_Now_Konnektor_Editor.src.controls
{
    /// <summary>
    /// Ein Steuerelement, dessen Items aus zwei Werten besteht.
    /// </summary>
    public partial class DictionaryControl : UserControl
    {
        private readonly ListControlHelper helper = new ListControlHelper();



        public DictionaryControl()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddItem(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                bool itemFound = false;
                foreach (PairValue item in ItemsListbox.Items)
                {
                    if (item.Key.Equals(key))
                    {
                        item.Value = value;
                        itemFound = true;
                        break;
                    }
                }
                if (!itemFound)
                {
                    PairValue item = new PairValue(key, value);
                    ItemsListbox.Items.Add(item);
                    ItemsListbox.ScrollIntoView(item);
                }


            }
        }



        /// <summary>
        /// 
        /// </summary>
        public void AddItem()
        {
            AddItem(inputKeyTextbox.Text, inputValueTextbox.Text);
            inputKeyTextbox.Text = "";
            inputValueTextbox.Text = "";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }



        /// <summary>
        /// Wenn in einer Eingabe-Textbox die Enter-Taste gedrückt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddItem();
            }
        }



        /// <summary>
        /// Wenn auf den Löschen-Button gedrückt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            helper.RemoveItem(sender, ItemsListbox);
        }



        /// <summary>
        /// Wenn auf den Bearbeiten-Button gedrückt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            helper.EditItem(sender, ItemsListbox);
        }



        /// <summary>
        /// Wenn sich die Maus ein Item bewegt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            helper.GridMouseInteraction(sender);
        }



        /// <summary>
        /// Wenn die Maus ein Item verlässt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            helper.GridMouseInteraction(sender);
        }



        /// <summary>
        /// Wenn in einer Textbox die Enter-Taste gedrückt wird, werden die Textboxes nicht editierbar gemacht. Außerdem wird der Wert aus den Textboxes in das Item übernommen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Grid parentGrid = helper.GetItemGrid(sender);
                helper.SwitchTextboxEditable(parentGrid);
                helper.ChangeItemValue(helper.LastItemGrid, ItemsListbox, helper.CurrentItemIndex);
            }
        }



        /// <summary>
        /// Wenn die Auswahl sich verändert, werden alle Textboxes nicht editierbar gemacht. Außerdem wird der Wert aus den Textboxes in das Item übernommen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            helper.SwitchTextboxEditable(helper.LastItemGrid);
            helper.ChangeItemValue(helper.LastItemGrid, ItemsListbox, helper.CurrentItemIndex);
        }
    }
}
