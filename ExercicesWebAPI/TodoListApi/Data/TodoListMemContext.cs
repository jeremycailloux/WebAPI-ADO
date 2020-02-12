using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListApi.Models;

namespace TodoListApi.Data
{
    public class TodoListMemContext
    {
        private static List<Tache> _taches;
        static TodoListMemContext()
        {

            _taches = new List<Tache>();

            for (int i = 0; i < 4; i++)
            {
                _taches.Add(new Tache(i, i + " tache",  DateTime.Today.AddDays(i), DateTime.Today.AddDays(i+1), i, false)) ;
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

        public void ModifierTache(int id, Tache tache)
        {
            for (int i = 0; i < _taches.Count; i++)
            {
                if (_taches[i].Id == id)
                {
                    _taches[i] = tache;
                    break;
                }
            }
        }

        public void SupprimerTache(int id)
        {
            var t = _taches.FirstOrDefault(t => t.Id == id); //Renvoie la tache si trouvé sinon null

            if(t != null)
            {
                _taches.Remove(t);
            }
       
        }
    }
}
