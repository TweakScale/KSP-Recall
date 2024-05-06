/*
	This file is part of Attached, a component of KSP-Recall
		© 2020-2024 LisiasT : http://lisias.net <support@lisias.net>

	KSP-Recall is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	KSP-Recall is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with KSP-Recall. If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with KSP-Recall. If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections.Generic;
using UnityEngine;

namespace KSP_Recall { namespace Resourcefull
{
	internal class Pool
	{
		internal struct Resource_t
		{
			[SerializeField] private PartResource node;

			internal PartResource ToPartResource(Part part)
			{
				PartResource r = new PartResource(part);
				r.Copy(this.node);
				return r;
			}

			internal static Resource_t From(PartResource pr)
			{
				Resource_t r = new Resource_t();
				r.node = pr;
				return r;
			}

			internal PartResource Info => this.node;
		}

		internal class Resource_List
		{
			private readonly Dictionary<int, List<Resource_t>> map = new Dictionary<int, List<Resource_t>>();
			private readonly object MUTEX = new object();

			public void Clear(Part part)
			{
				lock(MUTEX) this.map[part.GetInstanceID()].Clear();
			}

			public void Destroy(Part part)
			{
				lock(MUTEX) this.map.Remove(part.GetInstanceID());
			}

			public List<Resource_t> Get(Part part)
			{
				lock(MUTEX) return new List<Resource_t>(this._list(part));
			}

			public int Count(Part part)
			{
				lock(MUTEX)
				{
					return this.map.ContainsKey(part.GetInstanceID())
						? this.map[part.GetInstanceID()].Count
						: 0
					;
				}
			}

			public bool HasSomething(Part part) => this.map.ContainsKey(part.GetInstanceID());

			internal void Copy(Part from, Part to)
			{
				lock(MUTEX)
				{
					List<Resource_t> l = this._list(to);
					l.Clear();
					l.AddRange(this._list(from));
				}
			}

			internal void Update(Part part)
			{
				lock(MUTEX)
				{
					this._list(part).Clear();
					foreach (PartResource pr in part.Resources)
						RESOURCES._list(part).Add(Resource_t.From(pr));
				}
			}

			internal void Restore(Part part)
			{
				part.Resources.Clear();
				foreach (Resource_t resource in this._list(part))
					part.Resources.Add(resource.ToPartResource(part));
			}

			private List<Resource_t> _list(Part part)
			{
				if (!this.map.ContainsKey(part.GetInstanceID())) this.map[part.GetInstanceID()] = new List<Resource_t>();
				return this.map[part.GetInstanceID()];
			}

		}

		internal static readonly Resource_List RESOURCES = new Resource_List();
	}
} }
