using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using conseilMoi.Resources.MaBase;

using System.IO;
using Mono.Data.Sqlite;
using conseilMoi.Resources.Classes;

namespace conseilMoi.Classes
{
    public class ExpandableListViewAdapter : BaseExpandableListAdapter
    {
       private  MaBase db = new MaBase();
        private Context context;

        private List<string> listGroup;
        private Dictionary<string, List<string>> lstChild;

        public ExpandableListViewAdapter(Context context, List<string> listGroup, Dictionary<string,List<string>>lstChild)
        {
            this.context = context;
            this.listGroup = listGroup;
            this.lstChild = lstChild;
        }


        public override int GroupCount
        {
            get
            {
                return listGroup.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if(convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.item_layout, null);
            }
           
            db.ExistBase();

            CheckBox textViewItem = convertView.FindViewById<CheckBox>(Resource.Id.item);
            string result = (string)GetChild(groupPosition, childPosition);

            string content ="";
            string check;

            string[] words = result.Split(' ');
            try
            {
                content = words[0] + " " + words[2] + " " + words[3];
                textViewItem.Text = content;
                check = words[1];
            }

            catch
            {
                content = words[0];
                check = "nocheck";
                textViewItem.Text = content;
            }

            if(words[0] == "0") { textViewItem.Clickable = false; /*textViewItem.SetCursorVisible(false); textViewItem.SetWidth(1); textViewItem.SetHeight(1);*/ }

            if (check == "check") { textViewItem.Checked = true; }
            else { textViewItem.Checked = false; }

            textViewItem.Click += delegate {
                textViewItem.Text += "T";

             //  if (textViewItem.Checked == true) { db.InsertProfilUtilisateur(words[0]); }
              // if(textViewItem.Checked == false && words[2] != "0") { db.DeleteProfilUtilisateur(words[0], words[2], words[3]); }

            };

            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.group_item, null);
            }
            string textGroup = (string)GetGroup(groupPosition);
            TextView textViewGroup = convertView.FindViewById<TextView>(Resource.Id.group);
            textViewGroup.Text = textGroup;
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}