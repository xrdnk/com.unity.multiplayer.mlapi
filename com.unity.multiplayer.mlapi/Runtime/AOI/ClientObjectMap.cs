using System;
using System.Collections.Generic;

using MLAPI.Connection;

namespace MLAPI.AOI
{
   public class _ClientObjectMap<CLIENT, OBJECT>
   {
      private _ClientObjectMapNode<CLIENT, OBJECT> root;

      public _ClientObjectMap()
      {
          root = new _ClientObjectMapNode<CLIENT, OBJECT>();
      }

      public HashSet<OBJECT> QueryFor(CLIENT client, _ClientObjectMapNode<CLIENT, OBJECT>.ObjMapTempDel myDel = null)
      {
          HashSet<OBJECT> results = new HashSet<OBJECT>();
          root.QueryFor(client, results, myDel);
          return results;
      }

      public void DespawnCleanup(OBJECT no)
      {
          root.DespawnCleanup(no);
      }
      public void AddNode(_ClientObjectMapNode<CLIENT, OBJECT> n)
      {
          root.AddNode(n);
      }

      private Dictionary<string, _ClientObjectMapNode<CLIENT, OBJECT>> nodeDictionary;
   }

    public class _ClientObjectMapNode<CLIENT, OBJECT>
    {
        public delegate void ObjMapTempDel(OBJECT obj);

        public delegate void DynamicQuery(CLIENT client, HashSet<OBJECT> results);

        public delegate void CleanupHandler(OBJECT obj);

        protected DynamicQuery dynamicQuery;
        protected CleanupHandler cleanupHandler;

        public _ClientObjectMapNode()
        {
            children = new List<_ClientObjectMapNode<CLIENT, OBJECT>>();
            alwaysRelevant = new HashSet<OBJECT>();
        }

        public void AddStatic(OBJECT o)
        {
            alwaysRelevant.Add(o);
        }

        public void DespawnCleanup(OBJECT o)
        {
            alwaysRelevant.Remove(o);
            if (cleanupHandler != null)
            {
                cleanupHandler(o);
            }

            foreach (var c in children)
            {
                c.DespawnCleanup(o);
            }
        }

        public void AddNode(_ClientObjectMapNode<CLIENT, OBJECT> newNode)
        {
            children.Add(newNode);
        }

        // override test me
        public void QueryFor(CLIENT client, HashSet<OBJECT> results, ObjMapTempDel del)
        {
            results.UnionWith(alwaysRelevant);
            foreach (var ar in alwaysRelevant)
            {
                del(ar);
            }

            if (dynamicQuery != null)
            {
                dynamicQuery(client, results);
            }
            foreach (var c in children)
            {
                c.QueryFor(client, results, del);
            }
        }

        public _ClientObjectMapNode<CLIENT, OBJECT> Next { get; set; }

        private _ClientObjectMapNode<CLIENT, OBJECT> next;
        private List<_ClientObjectMapNode<CLIENT, OBJECT>> children;

        private HashSet<OBJECT> alwaysRelevant;
    }


    public class ClientObjectMap : _ClientObjectMap<NetworkedClient, NetworkedObject>
    {
    }

    public class ClientObjectMapNode : _ClientObjectMapNode<NetworkedClient, NetworkedObject>
    {
    }
}
