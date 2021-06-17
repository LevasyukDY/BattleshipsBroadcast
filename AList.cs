using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipsBroadcast
{
    // просто расширенный класс списка для отлавливания событий при различных изменениях списка
    [Serializable]
    public class AList<T>: List<T>
    {

        public delegate void OnBeforeAddEvent(T item);
        public delegate void OnAfterAddEvent(T item);
        public delegate void OnBeforeRemoveEvent(T item);
        public delegate void OnAfterRemoveEvent(T item);

        public event OnBeforeAddEvent BeforeAddEvent;
        public event OnAfterAddEvent AfterAddEvent;
        public event OnBeforeRemoveEvent BeforeRemoveEvent;
        public event OnAfterRemoveEvent AfterRemoveEvent;

        public new void Add(T item)
        {
            BeforeAddEvent?.Invoke(item);
            base.Add(item);
            AfterAddEvent?.Invoke(item);
        }

        public new void Remove(T item)
        {
            BeforeRemoveEvent?.Invoke(item);
            base.Remove(item);
            AfterRemoveEvent?.Invoke(item);
        }

        public bool IsExists(T item)
        {
            foreach (T e in this)
            {
                if (e.Equals(item) == true) {
                    return true;
                }
            }
            return false;
        }

        public AList<T> Clone()
        {
            AList<T> Temp = new AList<T>();
            foreach (T e in this)
            {
                Temp.Add(e);
            }
            return Temp;
        }

    }
}
