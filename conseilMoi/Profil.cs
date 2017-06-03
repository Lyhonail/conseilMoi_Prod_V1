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
using conseilMoi.Resources.MaBase;
using conseilMoi.Classes;
//using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace conseilMoi
{
    [Activity(Label = "Profil")]
    public class Profil : Activity
    {
       // ListView lstData;
        List<Profils> lstSource = new List<Profils>();
        MaBase db = new MaBase();

        ExpandableListViewAdapter mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();

        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            db.ExistBase(this);

            SetContentView(Resource.Layout.Profil);

            // var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // SetSupportActionBar(toolbar);
            //SupportActionBar(toolbar);


            expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListView);
            SetData(out mAdapter);
            expandableListView.SetAdapter(mAdapter);



            expandableListView.ChildClick += (s, e) =>
            {
                Toast.MakeText(this, "clicked: " + mAdapter.GetChild(e.GroupPosition, e.ChildPosition), ToastLength.Short).Show();

            };

            //lstData = FindViewById<ListView>(Resource.Id.listViewNomProfil);
            //LoadData();
            /*lstData.ItemClick += (s, e) =>
            {
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

                }

            };
            */
            MobileBarcodeScanner.Initialize(Application);

            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewProfilProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewProfilHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewProfilScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewProfilConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewProfilAvertissement);

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


        private void SetData(out ExpandableListViewAdapter mAdapter)
        {
            List<string> groupA = new List<string>();
            groupA.Add("A-1");
            groupA.Add("A-2");
            groupA.Add("A-3");

            List<string> groupB = new List<string>();
            groupB.Add("B-1");
            groupB.Add("B-2");
            groupB.Add("B-3");

            List<Profils> groupProfils = new List<Profils>();
            groupProfils = db.SelectNomProfil();
            /*foreach ()
            {

            } */
            foreach (Profils p in groupProfils)
            {

                string pid = p.GetIdProfil();
                group.Add(p.GetNomProfil());
                List<ProfilsStandards> groupeCriteres = new List<ProfilsStandards>();
                groupeCriteres = db.SelectcritereProfilstandard(pid);

                List<string> groupC = new List<string>();
                foreach (ProfilsStandards ps in groupeCriteres)
                {

                    groupC.Add(ps.GetidCritere());

                }
                dicMyMap.Add(p.GetNomProfil(), groupC);

            }

            mAdapter = new ExpandableListViewAdapter(this, group, dicMyMap);
        }

        private void LoadData()
        {
            lstSource = db.SelectNomProfil();
            var adapter = new ListViewAdapterProfil(this, lstSource);
            lstData.Adapter = adapter;
        }

    }
}