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
    class ProduitRecos
    {
        String id_famille;
        String id_produit;
        String product_name;
        String id_nutriment;
        String id_valeur;

        public void SetProduitsReco(String idf, String idp, String pn, String idn, String idv)
        {
            id_famille = idf;
            id_produit = idp;
            product_name = pn;
            id_nutriment = idn;
            id_valeur = idv;
        }
        public String GetidFamille()
        {
            return id_famille;
        }
        public String GetidProduit()
        {
            return id_produit;
        }
        public String GetproductName()
        {
            return product_name;
        }
        public String GetidNutriment()
        {
            return id_nutriment;
        }
        public String GetidValeur()
        {
            return id_valeur;
        }

    }
}