﻿using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HeroesModLoaderConfig.Styles.Controls.Enhanced
{
    /// <summary>
    /// Customized DataGridView with support for dragging items.
    /// </summary>
    class EnhancedDataGridView : DataGridView
    {
        /// <summary>
        /// Constructor for the custom class.
        /// </summary>
        public EnhancedDataGridView()
        {
            // Add wheel support
            this.MouseWheel += AnimatedDataGridView_MouseWheel;

            // Drag & Drop
            this.DragEnter += EnhancedDataGridView_DragEnter;
            this.MouseMove += EnhancedDataGridView_MouseMove;
            this.MouseDown += EnhancedDataGridView_MouseDown;
            this.DragOver += EnhancedDataGridView_DragOver;
            this.DragDrop += EnhancedDataGridView_DragDrop;
        }

        /////////////////////////////
        // Drag & Drop Implementation
        /////////////////////////////

        /// <summary>
        /// Allows/disables drag & drop reordering.
        /// </summary>
        [Category("| Custom Options"), Description("Allows reordering (for single row) via Drag + Drop & Ctrl+Scroll. Requires AllowDrop to be set to true.")]
        public bool ReorderingEnabled { get; set; }

        /// <summary>
        /// Stores the dimension of the listview row (rectangle) that is to be dragged. 
        /// </summary>
        private Rectangle dimensionsOfRowToDrag;

        /// <summary>
        /// Stores the individual cell that the user clicks to start the drag operation.
        /// </summary>
        private DataGridViewRow rowToDrag;

        /// <summary>
        /// Stores the row index where the row drag operation started from.
        /// </summary>
        public int DragRowIndex { get; set; }

        /// <summary>
        /// The entry point for the dragging handler.
        /// Obtains the row the mouse hovers above when the user clicks the left mouse button to initiate dragging.
        /// Obtains the amount of space mouse needs to move to start the dragging operation.
        /// </summary>
        private void EnhancedDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // Reset currently dragged rectangle.
            dimensionsOfRowToDrag = Rectangle.Empty;

            // Get the index of the item the mouse has hit below.
            var hittestInfo = this.HitTest(e.X, e.Y);

            // Check if the mouse is over an item in the DataGridView.
            // Otherwise do nothing.
            if (hittestInfo.RowIndex != -1 && hittestInfo.ColumnIndex != -1)
            {
                // Set row index for dragging.
                DragRowIndex = hittestInfo.RowIndex;

                // Assign row the user clicks to start drag operation. 
                rowToDrag = this.Rows[DragRowIndex];

                if (rowToDrag != null)
                {
                    // The DragSize indicates the size that the mouse can move 
                    // before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;

                    // e.X = X Mouse location in terms of screen coordinates.
                    // e.Y = Y Mouse location in terms of screen coordinates.

                    // e.X - dragSize.Width / 2 = Left side of rectangle

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    dimensionsOfRowToDrag = 
                    new Rectangle
                    (
                        // The location of the rectangle, top left corner.
                        new Point
                        (
                            e.X - (dragSize.Width), // Left Edge
                            e.Y - (dragSize.Height) // Top Edge
                        ), 

                        // Set the drag size.
                        new Size
                        (
                            dragSize.Width * 2, // Width to Right Edge
                            dragSize.Height * 2 // Width to Bottom Edge
                        )
                    );
                }
            }
        }

        /// <summary>
        /// When the user moves the mouse inside the DataGridView.
        /// Checks if the left mouse button is held and triggers a drag and drop operation if
        /// the mouse is outside of the calculated area for starting the drag operation.
        /// Triggers if the user is currently dragging the object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnhancedDataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            // Check for left mouse button press.
            // Check if drag/drop is enabled.
            if (e.Button.HasFlag(MouseButtons.Left) && ReorderingEnabled)
            {
                // If the mouse cursor `e` is outside of the rectangle &&
                // If there is a currently held rectangle.
                if (dimensionsOfRowToDrag != Rectangle.Empty && !dimensionsOfRowToDrag.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the row.                    
                    this.DoDragDrop(rowToDrag, DragDropEffects.Copy);
                }
            }
        }

        /// <summary>
        /// When user starts dragging into listview, set the drag effect to copy item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnhancedDataGridView_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        /// <summary>
        /// When the object is dragged inside the area of the bounds of the control. 
        /// Set drag effect to copy item.
        /// </summary>
        private void EnhancedDataGridView_DragOver(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }

        /// <summary>
        /// Occurs when the user releases the mouse button to complete the drag/drop operation.
        /// Checks the row index which overlaps the current selection, removing the
        /// original item and placing it inside at the new row index.
        /// </summary>
        /// <param name="sender">Nothing special in there.</param>
        /// <param name="e">Drag event arguments contain a copy of the original data we started dragging.</param>
        protected virtual void EnhancedDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations set in DragEventArgs e for this events are relative 
            // to the screen (and not the window), they must be converted to client coordinates.
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));

            // If the drag operation was a copy (our operation). 
            // then find the row the mouse currently overlaps and insert our
            // new row into that slot.

            // First the original is deleted, then is inserted into the
            // row index which overlaps the mouse at the end of the drag operation.
            if (e.Effect == DragDropEffects.Copy)
            {
                // Obtain the row to add.
                DataGridViewRow row = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));

                // Obtain the Hit Testing information to obtain row the mouse is currently above.
                var hitTest = this.HitTest(clientPoint.X, clientPoint.Y);

                // Check if the drag operation is hovering over a row.
                if (hitTest.ColumnIndex != -1 && hitTest.RowIndex != -1)
                {
                    // Remove the original row.
                    this.Rows.RemoveAt(DragRowIndex);

                    // Insert the original row at new index where the mouse hit.
                    this.Rows.Insert(hitTest.RowIndex, row);

                    // Set the currently selected row to dropped row.
                    this.Rows[hitTest.RowIndex].Selected = true;
                }
            }
        }

        ///////////////////////////
        // Scrolling Implementation
        ///////////////////////////

        /// <summary>
        /// Empty delegate for fiding events when user swaps two items by holding CTRL while scrolling mouse wheel.
        /// </summary>
        public delegate void OnWheelDelegate(int lastRow);

        /// <summary>
        /// Event fires when the user swaps two items by holding CTRL while scrolling the mouse wheel.
        /// </summary>
        public event OnWheelDelegate OnRowSwapped;

        /// <summary>
        /// Stores the current change from 0 to the current scrolled to position.
        /// </summary>
        private int currentScrollDelta = 0;

        /// <summary>
        /// The amount of scrolling necessary for the user to move to the next item.
        /// </summary>
        const int scrollSensitivity = 120;

        /// <summary>
        /// Custom keyboard scrolling implementation.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) { IncrementRowIndex(); }
            else if (e.KeyCode == Keys.Up) { DecrementRowIndex(); }
            else if (e.KeyCode == Keys.Left) { SelectFirstRow(); }
            else if (e.KeyCode == Keys.Right) { SelectLastRow(); }
        }

        /// <summary>
        /// Implement selection of the next/previous row with the use of scrolling
        /// using the mouse wheel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimatedDataGridView_MouseWheel(object sender, MouseEventArgs e)
        {
            // Append to the current Delta
            currentScrollDelta = currentScrollDelta + e.Delta;

            // Obtain the amount of lines to scroll.
            // MouseWheelScrollLines = User's set amount of lines to scroll as set in Windows' settings.
            int linesToScroll = SystemInformation.MouseWheelScrollLines * currentScrollDelta / scrollSensitivity;

            // Scroll either up or down.
            if (linesToScroll >= 1) // Scroll up
            {
                // If Control is Held & Reorder is Enabled, Reorder As Well as Changing Selection.
                if (ModifierKeys.HasFlag(Keys.Control) && ReorderingEnabled) { SwapRowUpwards(); }
                else { DecrementRowIndex(); }
                
                currentScrollDelta = 0;
            }
            else if (linesToScroll <= -1) // Scroll down
            {
                // If Control is Held & Reorder is Enabled, Reorder As Well as Changing Selection.
                if (ModifierKeys.HasFlag(Keys.Control) && ReorderingEnabled) { SwapRowDownwards(); }
                else { IncrementRowIndex(); }

                currentScrollDelta = 0;
            }
        }

        /// <summary>
        /// Increments the row index by one, if we are on the last item, the first item
        /// will be selected.
        /// </summary>
        private void IncrementRowIndex()
        {
            // Get Next Index
            int nextRow = SelectedCells[0].RowIndex + 1;

            if (nextRow > Rows.Count - 1)
            {
                Rows[0].Selected = true;
                FirstDisplayedScrollingRowIndex = 0;
            }
            else
            {
                Rows[nextRow].Selected = true;
                FirstDisplayedScrollingRowIndex = nextRow;
            }
        }

        /// <summary>
        /// Decrements the row index by one, if we are on the first item, the first item
        /// will be selected.
        /// </summary>
        private void DecrementRowIndex()
        {
            // Get Next Index
            int nextRow = SelectedCells[0].RowIndex - 1;

            if (nextRow < 0)
            {
                FirstDisplayedScrollingRowIndex = Rows.Count - 1;
                Rows[Rows.Count - 1].Selected = true;
            }
            else
            {
                FirstDisplayedScrollingRowIndex = nextRow;
                Rows[nextRow].Selected = true;
            }
        }

        /// <summary>
        /// Swaps the current selected row with the row that is above the current row.
        /// </summary>
        private void SwapRowUpwards()
        {
            // Get Index of Row Above
            int nextRowIndex = SelectedCells[0].RowIndex - 1;

            // Get current row.
            int currentRowIndex = SelectedCells[0].RowIndex;

            // Obtain current row.
            DataGridViewRow currentRow = this.Rows[currentRowIndex];

            // Select last row index.
            if (nextRowIndex < 0) // Swap first row with last row.
            {
                // Get rows.
                DataGridViewRow lastRow = this.Rows[Rows.Count - 1];

                // Remove first and last item.
                Rows.RemoveAt(currentRowIndex);
                Rows.RemoveAt(Rows.Count - 1);

                // Insert items in swapped order.
                Rows.Insert(currentRowIndex, lastRow);
                Rows.Add(currentRow);

                // Make the last row visible.
                FirstDisplayedScrollingRowIndex = Rows.Count - 1;
                Rows[Rows.Count - 1].Selected = true;
            }
            else
            {
                // Remove current item & insert above next item.
                Rows.RemoveAt(currentRowIndex);
                Rows.Insert(nextRowIndex, currentRow);

                // Make the last row visible.
                FirstDisplayedScrollingRowIndex = nextRowIndex;
                Rows[nextRowIndex].Selected = true;
            }

            // Invoke swap delegate
            OnRowSwapped?.Invoke(currentRowIndex);
        }

        /// <summary>
        /// Swaps the current selected row with the row that is below the current row.
        /// </summary>
        private void SwapRowDownwards()
        {
            // Get Next Index
            int nextRowIndex = SelectedCells[0].RowIndex + 1;

            // Get current row.
            int currentRowIndex = SelectedCells[0].RowIndex;

            // Get rows.
            DataGridViewRow currentRow = this.Rows[currentRowIndex];

            // If the row is the last row.
            if (nextRowIndex > Rows.Count - 1)
            {
                // Get rows.
                DataGridViewRow firstRow = this.Rows[0];

                // Remove last and first row.
                Rows.RemoveAt(currentRowIndex);
                Rows.RemoveAt(0);

                // Insert rows swapped.
                Rows.Insert(0, currentRow);
                Rows.Add(firstRow);

                // Show 1st row
                FirstDisplayedScrollingRowIndex = 0;
                Rows[0].Selected = true;
            }
            else
            { 
                // Remove last and first row.
                Rows.RemoveAt(currentRowIndex);
                Rows.Insert(nextRowIndex, currentRow);

                // Show next row.
                FirstDisplayedScrollingRowIndex = nextRowIndex;
                Rows[nextRowIndex].Selected = true;
            }

            // Invoke swap delegate
            OnRowSwapped?.Invoke(currentRowIndex);
        }

        /// <summary>
        /// Selects the first row of the DataGridView
        /// </summary>
        private void SelectFirstRow()
        {
            Rows[0].Selected = true;
            FirstDisplayedScrollingRowIndex = 0;
        }

        /// <summary>
        /// Selects the last row of the DataGridView
        /// </summary>
        private void SelectLastRow()
        {
            Rows[Rows.Count - 1].Selected = true;
            FirstDisplayedScrollingRowIndex = Rows.Count - 1;
        }
    }
}