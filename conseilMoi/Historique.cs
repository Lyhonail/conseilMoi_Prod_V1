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
using ZXing.Mobile;
using conseilMoi.Resources.Classes;
using conseilMoi.Resources.MaBase;
using Android.Graphics;
using Android.Util;

namespace conseilMoi
{
    [Activity(Label = "Historique")]
    public class Historique : Activity
    {
        //ListView lstData;
       // List<Historiques> lstSource = new List<Historiques>();
        MaBase db = new MaBase();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            db.ExistBase(this);
            SetContentView(Resource.Layout.Historique);

            //je recupère le LinearLayout qui contient le corp de la page et la liste de l'historique
            LinearLayout linearLayout = FindViewById<LinearLayout>(Resource.Id.listView);

            //Je récupère l'historique dans la base de données
            List<Historiques> listeHistorique = db.SelectHistorique();

            //Pour chaque historique présent, je l'affiche
            foreach(Historiques h in listeHistorique)
            {
                //Je créer le LinearLayout qui contiendra la ligne
                LinearLayout LN = new LinearLayout(this) { Id = 10 };
                var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                LN.Orientation = Orientation.Horizontal;
                LN.SetBackgroundColor(Color.LightGray);
                paramL.SetMargins(0,0,0,5);

                //J'ajoute le LinearLayout
                linearLayout.AddView(LN, paramL);

                //Je créer le textView qui contient le nom du produit
                TextView textView1 = new TextView(this) { Id = 1 };
                textView1.Text = h.GetNomProduit();
                var param1 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .8f);
                textView1.SetTextColor(Color.Black);
                textView1.SetTextSize(ComplexUnitType.Px, 22);
                textView1.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                LN.AddView(textView1, param1);

                TextView textView2 = new TextView(this) { Id = 2 };
                textView2.Text = h.Getdate().Substring(0, 10);
                var param2 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                param2.SetMargins(5, 0, 0, 0);
                textView2.SetTextColor(Color.Black);
                textView2.SetTextSize(ComplexUnitType.Px, 15);
                textView2.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                LN.AddView(textView2, param2);

                LN.Click += delegate
                {
                    //Intent garde la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    //produit.PutExtra("IDproduit", "2000000010281");
                    produit.PutExtra("IDproduit", h.GetIdProduit());
                    StartActivity(produit);

                };

            }

            /*
            //lstData = FindViewById<ListView>(Resource.Id.listView);
            LoadData();


            lstData.ItemClick += (s, e) => {
                for (int i = 0; i < lstData.Count; i++)
                {
                    if (e.Position == i)
                    {
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.DarkGray);
                    }
                    else
                    {
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }

                    //Bindind data 
                    var txtproduitid = e.View.FindViewById<TextView>(Resource.Id.textView1);
                    var txttypeid = e.View.FindViewById<TextView>(Resource.Id.textView2);
                    var txtnameproduit = e.View.FindViewById<TextView>(Resource.Id.textView4);
                    var txtdate = e.View.FindViewById<TextView>(Resource.Id.textView3);

                }

            };

            /*listView.setOnItemClickListener(new OnItemClickListener() {
    @Override
    public void onItemClick(AdapterView<?> parent, View view,
        int position, long id)
        {
            Toast.makeText(getApplicationContext(),
                "Click ListItem Number " + position, Toast.LENGTH_LONG)
                .show();
        }
    });*/



            //*****************************************************************************//
            MobileBarcodeScanner.Initialize(Application);

            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewHistoriqueProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewHistoriqueHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewHistoriqueScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewHistoriqueConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewHistoriqueAvertissement);


            menuProfil.Click += delegate
            {
                StartActivity(typeof(Profil));
            };
            menuHistorique.Click += delegate
            {
                StartActivity(typeof(Historique));
            };

            menuConseil.Click += delegate
            {
                StartActivity(typeof(Conseil));
            };

            //Clik sur le bouton scanner
            menuScanner.Click += async (sender, e) =>
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
                {
                    //Intent garde la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    produit.PutExtra("IDproduit", result.Text);
                    StartActivity(produit);
                }
                else { }
            };

            menuAvertissement.Click += delegate
            {
                StartActivity(typeof(Avertissement));
            };

        }

        private void LoadData()
        {
           // lstSource = db.SelectHistorique();
            //var adapter = new ListViewAdapterHistorique(this, lstSource);
           // lstData.Adapter = adapter;
        }
    }
}