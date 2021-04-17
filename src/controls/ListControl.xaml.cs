using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Enable_Now_Konnektor_Editor.src.controls
{
    /// <summary>
    /// Interaktionslogik für ListControl.xaml
    /// </summary>
    public partial class ListControl : UserControl
    {
        private readonly ListControlHelper helper = new ListControlHelper();



        public ListControl()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                ItemsListbox.Items.Add(item);
                ItemsListbox.ScrollIntoView(item);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public void AddItem()
        {
            AddItem(inputTextbox.Text);
            inputTextbox.Text = "";
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
        /// Wenn im Eingabefeld die Enter-Taste gedrückt wird, dann soll der Wert der Listbox hinzugefügt werden.
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
        /// Wenn der Löschen-Button gedrückt wird, soll das Element aus der Listbox entfernt werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            helper.RemoveItem(sender, ItemsListbox);
        }



        /// <summary>
        /// Aktionen, wenn in einem der Editier-Button gedrückt wird.
        /// <list type="number">
        /// <item>Noch editierbare Textboxes sollen nicht editierbar werden.</item>
        /// <item>Die Textboxes sollen editierbar werden.</item>
        /// <item>Das ItemGrid, in dem sich der Editier-Button befindet, wird für spätere Zwecke (siehe 1.) vermerkt.</item>
        /// <item>Der Wert wird auf das Item in der Listbox übertragen.</item>
        /// </list>
        /// </summary>
        /// <param name="sender">Der Editier-Button.</param>
        /// <param name="e"></param>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            helper.EditItem(sender, ItemsListbox);
        }



        /// <summary>
        /// Die Maus bewegt sich in das Grid eines Items hinein.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            helper.GridMouseInteraction(sender);
        }



        /// <summary>
        /// Die Maus verlässt das Grid eines Item.s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            helper.GridMouseInteraction(sender);
        }



        /// <summary>
        /// Aktionen, wenn in einem Textfeld die Enter-Taste gedrückt wird.
        /// <list type="number">
        /// <item>Die Textboxes sollen nicht editierbar werden.</item>
        /// </list>
        /// </summary>
        /// <param name="sender">Die Textbox, in der die Enter-Taste gedrückt wurde.</param>
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
        /// Aktionen, wenn in der Listbox ein anderes Element ausgewählt wird.
        /// <list type="number">
        /// <item>Die Textboxes der vorherigen Auswahl sollen nicht editierbar werden.</item>
        /// <item>Der Wert der Textbox wird auf das Item übertragen.</item>
        /// </list>
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
