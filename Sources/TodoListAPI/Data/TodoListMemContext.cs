using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Models;

namespace TodoListAPI.Data
{
   /// <summary>
   /// Contexte de données avec stockage statique en mémoire
   /// NB/ En production, utiliser une vraie base de données
   /// </summary>
   public class TodoListMemContext : ITodoListContext
   {
      // Liste des tâches (non thread safe)
      private static readonly List<Tache> _taches;

      // Constructeur statique
      static TodoListMemContext()
      {
         // On instancie la liste
         _taches = new List<Tache>();

         // On y ajoute 3 tâches arbitraires
         for (int t = 1; t <= 3; t++)
         {
            _taches.Add(new Tache
            {
               Id = t,
               DateCreation = DateTime.Today.AddDays(-t),
               DateEcheance = DateTime.Today.AddDays(-t+7),
               Description = $"Tâche {t}",
               Priorite = t
            });
         }
      }

      public List<Tache> GetTaches()
      {
         return _taches;
      }

      public void AjouterTache(Tache tache)
      {
         _taches.Add(tache);
      }

      public void SupprimerTache(int id)
      {
         var t = _taches.FirstOrDefault(t => t.Id == id);
         if (t != null) _taches.Remove(t);
      }

      public void ModifierTache(Tache tache)
      {
         var t = _taches.FirstOrDefault(t => t.Id == tache.Id);
         if (t != null)
         {
            _taches.Remove(t);
            AjouterTache(tache);
         }
      }
   }
}
