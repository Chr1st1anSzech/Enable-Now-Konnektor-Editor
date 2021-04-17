using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Enable_Now_Konnektor_Editor.src.controls
{
    internal class ListControlHelper
    {
        internal Grid LastItemGrid { get; set; }
        internal int CurrentItemIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        internal void EditItem(object sender, ListBox listBox)
        {
            SwitchTextboxEditable(LastItemGrid);

            Grid itemGrid = GetItemGrid(sender);
            string currentValue = GetItemValues(itemGrid)[0];
            CurrentItemIndex = GetIndexOfItem(listBox, currentValue);
            SwitchTextboxEditable(itemGrid);
            ChangeItemValue(itemGrid, listBox, CurrentItemIndex);

            LastItemGrid = itemGrid;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        internal void RemoveItem(object sender, ListBox listBox)
        {
            Grid itemGrid = GetItemGrid(sender);
            string value = GetItemValues(itemGrid)[0];
            int index = GetIndexOfItem(listBox, value);
            if (index >= 0 && index < listBox.Items.Count)
            {
                listBox.Items.RemoveAt(index);
            }
        }



        /// <summary>
        /// Ermittelt den Index des Elements, das den übergebenen Wert hat.
        /// </summary>
        /// <param name="value">Der Wert, der in den Listbox-Items wiedergefunden werden soll.</param>
        /// <returns>Der Index des Wertes.</returns>
        internal int GetIndexOfItem(ListBox listBox, string value)
        {
            int length = listBox.Items.Count;
            for (int i = 0; i < length; i++)
            {
                if (listBox.Items[i].Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }



        /// <summary>
        /// Das ParentGrid des übergebenen Elements.
        /// </summary>
        /// <param name="element">Das Element, dessen übergeordnetes Grid ermittelt werden soll.</param>
        /// <returns></returns>
        internal Grid GetItemGrid(object sender)
        {
            if (sender is Grid grid)
            {
                return grid;
            }
            if (sender is FrameworkElement element && element.Parent is Grid parentGrid)
            {
                return parentGrid;
            }
            throw new ArgumentException("Es wurde kein Grid in diesem Item gefunden.");
        }



        /// <summary>
        /// Gibt den Inhalt der Textbox innerhalb des Grids zurück.
        /// </summary>
        /// <param name="parentGrid">Das Grid, aus dessen TextBox der Wert extrahiert werden soll.</param>
        /// <returns>Der Text der TextBox.</returns>
        internal string[] GetItemValues(Grid parentGrid)
        {
            IEnumerable<TextBox> textBoxes = parentGrid.Children.OfType<TextBox>();
            int length = textBoxes.Count();
            string[] values = new string[length];
            int index = 0;
            foreach (TextBox textBox in textBoxes)
            {
                values[index] = textBox.Text;
            }
            return values;
        }



        /// <summary>
        /// Aktionen, die beim Betreten und Verlassen eines Listbox-Items passieren.
        /// <list type="bullet">
        /// <item>Es werden die Buttons ein- bzw. ausgeblendet.</item>
        /// </list>
        /// </summary>
        /// <param name="sender"></param>
        internal void GridMouseInteraction(object sender)
        {
            Grid itemGrid = GetItemGrid(sender);
            SwitchButtonVisibility(itemGrid);
        }



        /// <summary>
        /// Der Wert aus dem übergebenen Grid
        /// </summary>
        /// <param name="itemGrid"></param>
        internal void ChangeItemValue(Grid itemGrid, ListBox listBox, int index)
        {
            if (itemGrid == null || index < 0 || index >= listBox.Items.Count) return;

            string value = GetItemValues(itemGrid)[0];
            listBox.Items[index] = value;
        }



        /// <summary>
        /// Bei allen Textboxes innerhalb des Grids Readonly und Focusable wechseln.
        /// </summary> 
        /// <param name="itemGrid">Das Grid, in dem die Änderung stattfinden soll.</param>
        internal void SwitchTextboxEditable(Grid itemGrid)
        {
            if (itemGrid == null) return;

            foreach (TextBox textBox in itemGrid.Children.OfType<TextBox>())
            {
                textBox.IsReadOnly = !textBox.IsReadOnly;
                textBox.Focusable = !textBox.Focusable;
            }

            LastItemGrid = null;
        }



        /// <summary>
        /// Bei allen Buttons innerhalb des Grids die Sichtbarkeit wechseln.
        /// </summary> 
        /// <param name="itemGrid">Das Grid, in dem die Änderung stattfinden soll.</param>
        internal void SwitchButtonVisibility(Grid itemGrid)
        {
            if (itemGrid == null) return;

            foreach (Button button in itemGrid.Children.OfType<Button>())
            {
                button.Opacity = (button.Opacity + 1) % 2;
            }
        }
    }
}
