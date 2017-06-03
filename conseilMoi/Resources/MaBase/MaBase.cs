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
using Mono.Data.Sqlite;
using System.IO;
using Android.Database.Sqlite;
using Android.Util;
using conseilMoi.Resources.Classes;
using conseilMoi.Classes;

namespace conseilMoi.Resources.MaBase
{
    class MaBase
    {
        //déclaration des variables de la classe
        String path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //chemin d'enregistrement de la base
        String maBase; //contient 'path' + le nom de la base - utilisé pour la connexion -
        SqliteConnection connexion; //variable de connexion à la base
                                    //Fin de déclarartion des variables


        public string ExistBase(Activity activity) //On vérifie si la base existe ( cette fonction est optionnelle car ne sert à rien d'autre)
        {
            try //si pas d'erreur
            {
                maBase = path + "/maBase.sqlite";
                //on regarde si le fichier existe déjà
                if (File.Exists(maBase)) { return maBase + " Existe déjà"; }
                else
                {
                    CreerBase(activity.Resources.OpenRawResource(Resource.Raw.data), maBase);
                    return maBase + " - à été créé";
                }
            }
            catch
            { //si erreur
                return " ERREUR";
            }
        }//fin creerBase

        public void CreerBase(Stream resStream, string basePath)
        {
            using (resStream)
            {
                using (FileStream f = new FileStream(basePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    int bytesCount = 1024;
                    byte[] bytes = new byte[bytesCount];
                    int bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    while (bytesReaded > 0)
                    {
                        f.Write(bytes, 0, bytesReaded);
                        bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    }
                }
            }
        }

        public void ReCreerBase(Stream resStream, string basePath)
        {
            using (resStream)
            {
                using (FileStream f = new FileStream(basePath, FileMode.Create, FileAccess.Write))
                {
                    int bytesCount = 1024;
                    byte[] bytes = new byte[bytesCount];
                    int bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    while (bytesReaded > 0)
                    {
                        f.Write(bytes, 0, bytesReaded);
                        bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    }
                }
            }
        }


        public void ConnexionOpen() // Ouverture de la connexion (si la base n'existe pas elle est automatiquement créée)
        {
            this.connexion = new SqliteConnection("Data Source=" + maBase + ";Version=3;");
            this.connexion.Open();
        }//fin ConnexionOpen

        public void ConnexionClose() //fermeture de la connexion
        {
            connexion.Close();
        }//fin ConnexionClose


        public string CreerTableUtilisateur()//création d'une table
        {
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "create table UTILISATEUR		" +
                                        " ( " +
                                        " ID_utilisateur integer PRIMARY KEY ASC, " +
                                        " login          text, " +
                                        " password       text, " +
                                        " nom            text, " +
                                        " prenom         text, " +
                                        " Adresse1       text, " +
                                        " Adresse2       text, " +
                                        " Ville          text, " +
                                        " CP             text, " +
                                        " TEL            text " +
                                        " ); ";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok creer table utilisateur";
            }
            //Retourne le message d'erreur SQL
            catch (SqliteException ex)
            {
                return ex.Message;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTable

        //création de la table PRofil
        public string CreerTableProfil()
        {
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "create table PROFIL		" +
                                        " ( " +
                                        " ID_profil PRIMARY KEY ASC, " +
                                        " Lib_profil          text, " +
                                        " ); ";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok creer table profil";
            }
            //Retourne le message d'erreur SQL
            catch (SqliteException ex)
            {
                return ex.Message;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil


        /*public String SelectIdProduittest()
        {
            this.ConnexionOpen();
            string sql = "select * from historique; ";
            SqliteCommand commanda = new SqliteCommand(sql, connexion);
            SqliteDataReader result = commanda.ExecuteReader();
            result.Read();

            return result.GetString(0).ToString();
        }*/

        //chargement du produit
        public Produits SelectIdProduit(String p, string IdTp)
        {
            try
            {
                this.ConnexionOpen();
                //Selection du produit
                string sql = "select id_produit, product_name, generic_name, image_small_url from produit where id_produit = " + p + "; ";
                SqliteCommand commanda = new SqliteCommand(sql, connexion);
                SqliteDataReader result = commanda.ExecuteReader();
                result.Read();
                Produits produits = new Produits();
                produits.SetProduits(result.GetString(0), result.GetString(1), result.GetString(2));
                produits.SetUrl(result.GetString(3));
                result.Close();

                //recherche de tous les allergenes qui composent le produit
                string sql_allergene = "select id_allergene from compo_allergene where id_produit  = '" + p + "'; ";
                SqliteCommand command_allergene = new SqliteCommand(sql_allergene, connexion);
                SqliteDataReader result_allergene = command_allergene.ExecuteReader();
                while (result_allergene.Read())
                {
                    produits.AddAllergene(result_allergene.GetInt32(0).ToString());
                    //produits.AddAllergene("100");
                }
                result_allergene.Close();

                //recherche de tous les nutriments qui composent le produit
                string sql_nutriment = "select id_nutriment from compo_nutriment where id_produit  = '" + p + "'; ";
                SqliteCommand command_nutriment = new SqliteCommand(sql_nutriment, connexion);
                SqliteDataReader result_nutriment = command_nutriment.ExecuteReader();
                while (result_nutriment.Read())
                {
                    produits.AddNutriment(result_nutriment.GetString(0));
                }
                result_nutriment.Close();

                /* MATCH ALLERGENE */
                //Recherche d'allergène qui matchent avec les critères du profil
                string sql_recherche_allergene =
                    " select PU.ID_typeProfil, PU.ID_profil, PU.ID_critere " +
                    " from profil_utilisateur PU, compo_allergene CA " +
                    " where PU.ID_critere = CA.ID_allergene " +
                    " AND CA.id_produit = '" + p + "' AND PU.ID_typeProfil='"+IdTp+"' ; ";
                SqliteCommand command_recherche_allergene = new SqliteCommand(sql_recherche_allergene, connexion);
                SqliteDataReader result_recherche_allergene = command_recherche_allergene.ExecuteReader();
                while (result_recherche_allergene.Read())
                {
                    produits.AddCheckAllergene(result_recherche_allergene.GetString(2).ToString(),
                                               result_recherche_allergene.GetString(0).ToString(),
                                               result_recherche_allergene.GetString(1).ToString());
                }
                //Fin recherche d'allergene
                result_recherche_allergene.Close();
                /* FIN MATCH ALLERGENE */

                /* MATCH NUTRIMENT */
                //Recherche de NUTRIMENTS qui matchent avec les critères du profil
                string sql_recherche_nutriment =
                    " select id_typeProfil, id_profil, id_nutriment, PU.valeur, CN.valeur, seuil_vert, seuil_orange, seuil_rouge " +
                    "   from profil_utilisateur PU, compo_nutriment CN " +
                    "  where id_produit = '" + p + "' AND PU.ID_typeProfil='" + IdTp + "' " +
                    " AND ID_critere = id_nutriment; ";
                SqliteCommand command_recherche_nutriment = new SqliteCommand(sql_recherche_nutriment, connexion);
                SqliteDataReader result_recherche_nutriment = command_recherche_nutriment.ExecuteReader();
                while (result_recherche_nutriment.Read())
                {
                    String id_typeProfil = result_recherche_nutriment.GetString(0);
                    String id_profil = result_recherche_nutriment.GetString(1);
                    String id_nutriment = result_recherche_nutriment.GetString(2);
                    //decimal valeur_profil = 10;
                    decimal valeur_profil = result_recherche_nutriment.GetDecimal(3);
                    decimal valeur_produit = decimal.Parse(result_recherche_nutriment.GetString(4));
                    //decimal valeur_produit = 10;
                     decimal vert = result_recherche_nutriment.GetDecimal(5);
                    //decimal vert = 10;

                    //  decimal vert = result_recherche_nutriment.GetDecimal(5);
                    decimal orange = result_recherche_nutriment.GetDecimal(6);
                    //decimal orange = 10;
                    decimal rouge = result_recherche_nutriment.GetDecimal(7);

                    /*
                    decimal valeur_profil = result_recherche_nutriment.GetDecimal(3);
                    decimal valeur_produit = decimal.Parse(result_recherche_nutriment.GetString(4));
                    decimal vert = result_recherche_nutriment.GetDecimal(5);
                    decimal orange = result_recherche_nutriment.GetDecimal(6);
                    decimal rouge = result_recherche_nutriment.GetDecimal(7);
                    */

                    produits.AddCheckNutriment(id_typeProfil, id_profil, id_nutriment, valeur_profil, valeur_produit, vert, orange, rouge);
                }
                //Fin recherche d'allergene
                result_recherche_nutriment.Close();
                /* FIN MATCH NUTRIMENT */



                //on retourne le produit en entier
                return produits;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                //pas de résultat, on va donc créer un produit vide qui renvoie l'information "aucun produit"
                Produits produits = new Produits();
                produits.SetProduits("000", "erreur", "erreur");
                return produits;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil


        //Scann d'un produit --> Ajout dans l'historique
        public string InsertIntoHistorique(String id_typeProfil, String id_produit)
        {
            //On tente d'abbord de modifier un enregistrement qui existerai déjà : la date et les produit  de substitution
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "insert into historique (id_typeProfil, id_produit, date) values ( '" + id_typeProfil + "', '" + id_produit + "', datetime() );";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok insert ";
            }
            //Si le couple id_produit et id_typeProfil n'existe pas = on le créer
            catch
            {
                try
                {
                    this.ConnexionOpen();
                    SqliteCommand command = connexion.CreateCommand();
                    command.CommandText = "update historique set  date = datetime() where id_typeProfil='" + id_typeProfil + "' and id_produit='" + id_produit + "' ";

                    command.ExecuteNonQuery();
                    connexion.Close();
                    return "update ";
                }
                catch
                {
                    return "Erreur update ";
                }

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil




        //chargement du produit
        public List<Historiques> SelectHistorique()
        {
            Historiques historiques = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sql = "select id_produit, date from historique; ";
                SqliteCommand commanda = new SqliteCommand(sql, connexion);
                SqliteDataReader result = commanda.ExecuteReader();
                List<Historiques> Hist = new List<Historiques>();

                while (result.Read())
                {
                    //Historiques historiques = new Historiques();

                    historiques = new Historiques();
                    //historiques.CreeHistorique(Int32.Parse(result.GetString(0)), result.GetString(1)) ;
                    String chaine = result.GetString(0);
                    String sqlNomProduit = "select p.product_name from produit p where p.id_produit = '" + chaine + "'; ";
                    SqliteCommand commandaNomProduit = new SqliteCommand(sqlNomProduit, connexion);
                    SqliteDataReader resultNomProduit = commandaNomProduit.ExecuteReader();
                    resultNomProduit.Read();
                    long num = long.Parse(chaine);
                    historiques.CreeHistorique(chaine, resultNomProduit.GetString(0), result.GetString(1));
                    //historiques.CreeHistorique(1, "test");
                    resultNomProduit.Close();
                    Hist.Add(historiques);
                }


                // historiques.id_produit(result.GetString(0).ToString(), result.GetString(1).ToString(), result.GetString(2).ToString());
                result.Close();
                return Hist;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<Historiques> Hist = new List<Historiques>();
                return Hist;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        //Créer la requete pour les produits recommandé

        //chargement du produit
        public List<Produits> SelectProduitRecommande()
        {
            Produits produitRecommande = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlRecommande = "select id_produit, product_name from produit where id_produit=10 or id_produit=1000008218 or id_produit=1000008706; ";
                SqliteCommand commandaReco = new SqliteCommand(sqlRecommande, connexion);
                SqliteDataReader result = commandaReco.ExecuteReader();
                List<Produits> ProduitRec = new List<Produits>();

                while (result.Read())
                {
                    //Historiques historiques = new Historiques();

                    produitRecommande = new Produits();
                    String chaine = result.GetString(0);
                    long num = long.Parse(chaine);
                    produitRecommande.SetProduits(chaine, result.GetString(1), "");
                    ProduitRec.Add(produitRecommande);
                }
                result.Close();
                return ProduitRec;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<Produits> ProduitRec = new List<Produits>();
                return ProduitRec;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public String SelectLibNutriment(String id)
        {
            String lib="";

            this.ConnexionOpen();
            //Selection de l'historique
            string sql = "select lib_nutriment from nutriment where id_nutriment = '"+id+"'; ";
            SqliteCommand command = new SqliteCommand(sql, connexion);
            SqliteDataReader result = command.ExecuteReader();

            result.Read();

            lib = result.GetString(0);

            return lib;
        }

        public List<Profils> SelectNomProfil()
        {
            Profils profilName = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlNomProfil = "select lib_profil, id_profil from profil; ";
                SqliteCommand commandaNomProfil = new SqliteCommand(sqlNomProfil, connexion);
                SqliteDataReader result = commandaNomProfil.ExecuteReader();
                List<Profils> profilNames = new List<Profils>();
                //List<string> profilListExpensible;
                while (result.Read())
                {
                    
                    //Historiques historiques = new Historiques();
                    profilName = new Profils();
                    String chaine = result.GetString(0);
                    String chaine1 = result.GetString(1);
                    profilName.CreeProfil(chaine, chaine1);
                    profilNames.Add(profilName);
                }
                result.Close();
                return profilNames;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<Profils> profilNames = new List<Profils>();
                return profilNames;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public String GetLibNutriment(String ID_Nut)
        {
            this.ConnexionOpen();
            //Selection du libellé
            string sqlLibNut = "select lib_nutriment from nutriment where id_nutriment = '"+ID_Nut+"'; ";
            SqliteCommand commandLibNut = new SqliteCommand(sqlLibNut, connexion);
            SqliteDataReader result = commandLibNut.ExecuteReader();
            result.Read();
            string rez = result.GetString(0);
            result.Close();
            this.ConnexionClose();

            return rez;
        }

        public String GetLibAllergene(String ID_All)
        {
            this.ConnexionOpen();
            //Selection du libellé
            string sqlLibAll = "select lib_allergene from allergene where ID_allergene = " + ID_All + "; ";
            SqliteCommand commandLibAll = new SqliteCommand(sqlLibAll, connexion);
            SqliteDataReader result = commandLibAll.ExecuteReader();
            result.Read();

            string rez = result.GetString(0);
            result.Close();
            this.ConnexionClose();

            return rez;
        }

        public List<ProfilsStandards> SelectcritereProfilstandard(String idc)
        {
            ProfilsStandards profilcritere = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlNomProfil = "select id_profil, id_critere, valeur, seuil_vert, seuil_orange, seuil_rouge from profil_standard where id_profil = '" + idc + "'; ";
                SqliteCommand commandaNomProfilcritere = new SqliteCommand(sqlNomProfil, connexion);
                SqliteDataReader result = commandaNomProfilcritere.ExecuteReader();
                List<ProfilsStandards> profilcriteres = new List<ProfilsStandards>();
                //List<string> profilListExpensible;
                while (result.Read())
                {

                    //Historiques historiques = new Historiques();
                    profilcritere = new ProfilsStandards();
                    String chaine = result.GetString(0);
                    String chaine1 = result.GetString(1);
                    String chaine2 = result.GetString(0);
                    String chaine3 = result.GetString(1);
                    String chaine4 = result.GetString(0);
                    String chaine5 = result.GetString(1);
                    profilcritere.CreeProfilStandard(chaine, chaine1, chaine2, chaine3, chaine4, chaine5);
                    profilcriteres.Add(profilcritere);
                }
                result.Close();
                return profilcriteres;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<ProfilsStandards> profilcriteres = new List<ProfilsStandards>();
                return profilcriteres;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }


        /*public string InsertAllProfilUtilisateur(String id_Profil)
        {
            //On tente d'abbord de modifier un enregistrement qui existerai déjà : la date et les produit  de substitution
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "insert into profil_utilisateur (ID_profil, ID_critere, valeur,SEUIL_VERT, SEUIL_ORANGE,SEUIL_ROUGE) values ( '" + id_typeProfil + "', '" + id_produit + "', datetime() );";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok insert ";
            }
            //Si le couple id_produit et id_typeProfil n'existe pas = on le créer
            catch
            {
 
                    return "Erreur insert ";
        

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil


    */




















    }
}