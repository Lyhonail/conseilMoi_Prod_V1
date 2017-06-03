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

namespace conseilMoi.Classes
{

    //public TextView txtNomTypeProfil { get; set; }


    public class ListViewAdapterProfil : BaseAdapter
    {
        public TextView txtNomTypeProfil { get; set; }
        private Activity activity;
        private List<Profils> lstProfil;

        public ListViewAdapterProfil(Activity activity, List<Profils> lstProfil)
        {
            this.activity = activity;
            this.lstProfil = lstProfil;

        }

        public override int Count
        {
            get
            {
                return lstProfil.Count();
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return long.Parse(lstProfil[position].GetNomProfil());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ListViewDataProfil, parent, false);
            var txtNomTypeProfil = view.FindViewById<TextView>(Resource.Id.textViewNomProfil);
            txtNomTypeProfil.Text = "" + lstProfil[position].GetNomProfil();
            return view;
        }
    }

}