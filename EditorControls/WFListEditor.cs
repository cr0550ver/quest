﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AxeSoftware.Quest.EditorControls
{
    public interface IListEditorDelegate
    {
        void DoAdd();
        void DoEdit(string key, int index);
        void DoRemove(string[] keys);
    }

    public partial class WFListEditor : UserControl
    {
        public WFListEditor()
        {
            InitializeComponent();

            Resize += ListControl_Resize;
            cmdAdd.Click += cmdAdd_Click;
            cmdDelete.Click += cmdDelete_Click;
            cmdEdit.Click += cmdEdit_Click;
            lstList.DoubleClick += lstList_DoubleClick;
            lstList.ItemSelectionChanged += lstList_ItemSelectionChanged;
            lstList.SelectedIndexChanged += lstList_SelectedIndexChanged;
            Style = ColumnStyle.OneColumn;
            UpdateList(null);
        }

        public enum ColumnStyle
        {
            OneColumn,
            TwoColumns
        }

        private Dictionary<string, ListViewItem> m_listItems = new Dictionary<string, ListViewItem>();
        private IListEditorDelegate m_delegate;
        private ColumnStyle m_style;
        private string[] m_headers = new string[3];

        private bool m_readOnly;

        // This event is triggered as soon as any toolbar button is clicked, so that the parent
        // can save any existing updates. This is necessary because the TextBoxControl will only
        // cause an update when it detects that it has lost focus, but this won't happen when a
        // toolbar button is clicked, so we need to force it.
        public event ToolbarClickedEventHandler ToolbarClicked;
        public delegate void ToolbarClickedEventHandler();

        public void UpdateList(IEnumerable<KeyValuePair<string, string>> list)
        {
            lstList.Clear();
            m_listItems.Clear();
            InitialiseColumnHeaders();
            int index = 0;

            if (list != null)
            {
                foreach (var item in list)
                {
                    AddListItem(item, index);
                    index += 1;
                }
            }

            ResizeListEditor();
            SetButtonsEnabledStatus();
        }

        private void InitialiseColumnHeaders()
        {
            if (Style == ColumnStyle.OneColumn)
            {
                ColumnHeader mainColumn = new ColumnHeader();
                lstList.Columns.Add(mainColumn);
                mainColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                lstList.HeaderStyle = ColumnHeaderStyle.None;
                lstList.FullRowSelect = false;
            }
            else
            {
                ColumnHeader column1 = new ColumnHeader();
                column1.Text = m_headers[1];
                ColumnHeader column2 = new ColumnHeader();
                column2.Text = m_headers[2];
                lstList.Columns.Add(column1);
                lstList.Columns.Add(column2);
                lstList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                lstList.FullRowSelect = true;
            }
        }

        public void AddListItem(KeyValuePair<string, string> item, int index)
        {
            ListViewItem newListViewItem = default(ListViewItem);
            ListViewItem lvItem;

            switch (Style)
            {
                case ColumnStyle.OneColumn:
                    lvItem = new ListViewItem(EditorUtility.FormatAsOneLine(item.Value));
                    lvItem.Name = item.Key;
                    newListViewItem = lstList.Items.Insert(index, lvItem);
                    break;
                case ColumnStyle.TwoColumns:
                    lvItem = new ListViewItem(EditorUtility.FormatAsOneLine(item.Key));
                    lvItem.Name = item.Key;
                    newListViewItem = lstList.Items.Insert(index, lvItem);
                    newListViewItem.SubItems.Add(EditorUtility.FormatAsOneLine(item.Value));
                    break;
                default:
                    throw new InvalidOperationException("Invalid column style");
            }

            m_listItems.Add(item.Key, newListViewItem);
        }

        public void RemoveListItem(string key)
        {
            lstList.Items.Remove(m_listItems[key]);
            m_listItems.Remove(key);
        }

        private void ListControl_Resize(object sender, System.EventArgs e)
        {
            ResizeListEditor();
        }

        private void ResizeListEditor()
        {
            if (lstList.Columns.Count == 0) return;

            switch (Style)
            {
                case ColumnStyle.OneColumn:
                    lstList.Columns[0].Width = lstList.Width - SystemInformation.VerticalScrollBarWidth - lstList.Margin.Horizontal;
                    break;
                case ColumnStyle.TwoColumns:
                    lstList.Columns[1].Width = lstList.Width - SystemInformation.VerticalScrollBarWidth - lstList.Margin.Horizontal - lstList.Columns[0].Width;
                    break;
                default:
                    throw new InvalidOperationException("Invalid column style");
            }
        }

        private void cmdAdd_Click(System.Object sender, System.EventArgs e)
        {
            if (m_readOnly) return;
            if (ToolbarClicked != null)
            {
                ToolbarClicked();
            }
            m_delegate.DoAdd();
        }

        private void cmdDelete_Click(object sender, System.EventArgs e)
        {
            if (m_readOnly) return;
            if (ToolbarClicked != null)
            {
                ToolbarClicked();
            }
            m_delegate.DoRemove(GetSelectedItems().ToArray());
        }

        private void cmdEdit_Click(System.Object sender, System.EventArgs e)
        {
            if (m_readOnly) return;
            if (ToolbarClicked != null)
            {
                ToolbarClicked();
            }
            EditSelectedItem();
        }

        private void EditSelectedItem()
        {
            m_delegate.DoEdit(lstList.SelectedItems[0].Name, lstList.SelectedItems[0].Index);
        }

        private List<string> GetSelectedItems()
        {
            List<string> result = new List<string>();
            foreach (ListViewItem item in lstList.SelectedItems)
            {
                result.Add(item.Name);
            }
            return result;
        }

        private void lstList_DoubleClick(object sender, System.EventArgs e)
        {
            if (IsEditAllowed()) EditSelectedItem();
        }

        private void lstList_ItemSelectionChanged(object sender, System.Windows.Forms.ListViewItemSelectionChangedEventArgs e)
        {
            SetButtonsEnabledStatus();
        }

        private void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetButtonsEnabledStatus();
        }

        private void SetButtonsEnabledStatus()
        {
            cmdEdit.Enabled = IsEditAllowed();
            cmdDelete.Enabled = IsDeleteAllowed();
        }

        private bool IsEditAllowed()
        {
            if (m_readOnly) return false;
            return lstList.SelectedItems.Count == 1;
        }

        private bool IsDeleteAllowed()
        {
            if (m_readOnly) return false;
            return lstList.SelectedItems.Count > 0;
        }

        public IListEditorDelegate EditorDelegate
        {
            get { return m_delegate; }
            set { m_delegate = value; }
        }

        public void SetSelectedItem(string key)
        {
            lstList.SelectedItems.Clear();
            m_listItems[key].Selected = true;
            m_listItems[key].Focused = true;
            m_listItems[key].EnsureVisible();
            lstList.Focus();
        }

        public ColumnStyle Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public void SetHeader(int column, string text)
        {
            m_headers[column] = text;
        }

        public void UpdateValue(int index, string text)
        {
            lstList.Items[index].SubItems[1].Text = EditorUtility.FormatAsOneLine(text);
        }

        public bool IsReadOnly
        {
            get { return m_readOnly; }
            set
            {
                m_readOnly = value;
                cmdAdd.Enabled = !m_readOnly;
            }
        }
    }
}