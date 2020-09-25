using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Utility.WindowsForms.CustomControls.ToolStripPaths
{
    public static class ToolStripExtensions
    {

        private static ToolStripMenuItem CreateItem(string name)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem();
            tsmi.Name = name;
            tsmi.Size = new Size(180, 22);
            tsmi.Text = name;
            return tsmi;
        }

        public static void RemoveItem(this ToolStrip dd, string path)
        {
            dd.Items.RemoveItem(path);
        }

        public static void RemoveItem(this ToolStripDropDownItem dd, string path)
        {
            dd.DropDownItems.RemoveItem(path);
        }

        public static void RemoveItem(this ToolStripItemCollection menu, string path)
        {
            string[] parts = path.Split('/', '\\');
            ToolStripItemCollection current = menu;
            for (int i = 0; i < parts.Length; i++)
            {
                bool isLast = i == parts.Length - 1;
                if (current.ContainsKey(parts[i]))
                {
                    ToolStripItem item = current[parts[i]];
                    if (isLast)
                    {
                        current.Remove(item);
                        item.Dispose();
                    }

                    if (item is ToolStripDropDownItem mitem)
                    {
                        current = mitem.DropDownItems;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public static ToolStripDropDownItem GetItem(
            this ToolStrip dd, string path, Action<ToolStripDropDownItem> onCreate = null,
            List<ToolStripDropDownItem> addList = null)
        {
            return dd.Items.GetItem(path, onCreate, addList);
        }

        public static ToolStripDropDownItem GetItem(
            this ToolStripDropDownItem dd, string path, Action<ToolStripDropDownItem> onCreate = null,
            List<ToolStripDropDownItem> addList = null)
        {
            return dd.DropDownItems.GetItem(path, onCreate, addList);
        }

        public static ToolStripDropDownItem GetItem(
            this ToolStripItemCollection menu, string path, Action<ToolStripDropDownItem> onCreate = null,
            List<ToolStripDropDownItem> addList = null)
        {
            string[] parts = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            ToolStripItemCollection current = menu;
            for (int i = 0; i < parts.Length; i++)
            {
                bool isLast = i == parts.Length - 1;
                if (current.ContainsKey(parts[i]))
                {
                    ToolStripItem item = current[parts[i]];
                    if (item is ToolStripDropDownItem mitem)
                    {
                        current = mitem.DropDownItems;
                    }
                    else
                    {
                        mitem = CreateItem(parts[i]);
                        addList?.Add(mitem);
                        onCreate?.Invoke(mitem);
                        current.Add(mitem);
                        current = mitem.DropDownItems;
                    }

                    if (isLast)
                    {
                        return mitem;
                    }
                }
                else
                {
                    ToolStripDropDownItem item = CreateItem(parts[i]);
                    addList?.Add(item);
                    onCreate?.Invoke(item);
                    current.Add(item);
                    current = item.DropDownItems;
                    if (isLast)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

    }
}