﻿using System;
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
    class Nutriment
    {
        String ID_nutriment;
        String ID_typeProfil;
        String ID_Profil;
        decimal valeur_produit;
        decimal valeur_profil;
        decimal seuil_vert;
        decimal seuil_orange;
        String impact;

        public void CreeNutriment(String idtp, String idp, String idN, decimal val_prod, decimal val_prof, decimal vert, decimal orange)
        {
            ID_nutriment = idN;
            ID_typeProfil = idtp;
            ID_Profil = idp;
            valeur_produit = val_prod;
            valeur_profil = val_prof;
            seuil_vert = vert;
            seuil_orange = orange;
        }

        public String GetIdNutriment()
        {
            return ID_nutriment;
        }

        public String GetIdProfil()
        {
            return ID_Profil;
        }

        public String GetIdTypeProfil()
        {
            return ID_typeProfil;
        }

        public decimal GetValeurProfil()
        {
            return valeur_profil;
        }

        public decimal GetValeurProduit()
        {
            return valeur_produit;
        }

        public decimal GetVert()
        {
            return seuil_vert;
        }

        public decimal GetOrange()
        {
            return seuil_orange;
        }

        public void SetImpact(String i)
        {
            impact = i;
        }

        public String GetImpact()
        {
            return impact;
        }

        
    }
}