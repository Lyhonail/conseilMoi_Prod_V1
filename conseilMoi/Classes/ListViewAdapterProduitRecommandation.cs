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
using conseilMoi.Resources.Classes;

namespace conseilMoi.Classes
{
    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtIdProduitRecommande { get; set; }

        public TextView txtNameProduitRecommande  { get; set; }
    }

    class ListViewAdapterProduitRecommandation:BaseAdapter
    {
        private Activity activity;
        private List<Produits> lstProduitRecommandee;

        public ListViewAdapterProduitRecommandation(Activity activity, List<Produits> lstProduitRecommandee)
        {
            this.activity = activity;
            this.lstProduitRecommandee = lstProduitRecommandee;

        }

        public override int Count
        {
            get
            {
                return lstProduitRecommandee.Count();
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return long.Parse(lstProduitRecommandee[position].GetId_Produit());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ListViewDataProduitRecommande, parent, false);
            var txtIdProduitRecommande = view.FindViewById<TextView>(Resource.Id.textViewIdRecommande);
            var txtNameProduitRecommande = view.FindViewById<TextView>(Resource.Id.textViewNomRecommande);

            txtIdProduitRecommande.Text = "" + lstProduitRecommandee[position].GetId_Produit();
            txtNameProduitRecommande.Text = "" + lstProduitRecommandee[position].GetProduct_name();

            return view;
        }
    }
}
