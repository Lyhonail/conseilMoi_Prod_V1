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
    public class Profils
    {
        String idprofil;
        String nomProfil;
        Boolean selected;

        public void CreeProfil(String np, String ip)
        {
            nomProfil = np;
            idprofil = ip;
            //selected = false;
        }

        public String GetNomProfil()
        {
            return nomProfil;
        }

        public String GetIdProfil()
        {
            return idprofil;
        }

        public Boolean isSelected()
        {
            return selected;
        }

        public void setSelected(Boolean selected)
        {
            this.selected = selected;
        }

    }


}