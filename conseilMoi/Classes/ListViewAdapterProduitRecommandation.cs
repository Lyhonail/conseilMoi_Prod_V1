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
        public TextView txtIdFamille { get; set; }

        public TextView txtIdProduit { get; set; }

        public TextView txtproductname { get; set; }

        public TextView txtIdnutriment { get; set; }

        public TextView txtvaleur { get; set; }
    }

    class ListViewAdapterProduitRecommandation:BaseAdapter
    {
        private Activity activity;
        private List<ProduitRecos> lstProduitRecommandee;

        public ListViewAdapterProduitRecommandation(Activity activity, List<ProduitRecos> lstProduitRecommandee)
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
            return long.Parse(lstProduitRecommandee[position].GetidProduit());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ListViewDataProduitRecommande, parent, false);
            var txtIdFamille = view.FindViewById<TextView>(Resource.Id.textViewidfamille);
            var txtIdProduit = view.FindViewById<TextView>(Resource.Id.textViewidproduit);
            var txtproductname = view.FindViewById<TextView>(Resource.Id.textViewProductname);
            var txtIdnutriment = view.FindViewById<TextView>(Resource.Id.textViewIdNutriment);
            var txtvaleur = view.FindViewById<TextView>(Resource.Id.textViewvaleur);

            txtIdFamille.Text = "" + lstProduitRecommandee[position].GetidFamille();
            txtIdProduit.Text = "" + lstProduitRecommandee[position].GetidProduit();
            txtproductname.Text = "" + lstProduitRecommandee[position].GetproductName();
            txtIdnutriment.Text = "" + lstProduitRecommandee[position].GetidNutriment();
            txtvaleur.Text = "" + lstProduitRecommandee[position].GetidValeur();


            return view;
        }
    }
}
