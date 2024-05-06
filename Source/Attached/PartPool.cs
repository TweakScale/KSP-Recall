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

namespace KSP_Recall { namespace Attached
{
	internal class Pool
	{
		internal struct Attachment_t
		{
			[SerializeField] private AttachNode node;

			internal AttachNode ToPartAttachNode(Part part)
			{
				AttachNode r = new AttachNode(node.id, node.nodeTransform, node.size, node.attachMethod, node.ResourceXFeed, node.rigid);
				r.AllowOneWayXFeed = node.AllowOneWayXFeed;
				r.attachedPart = node.attachedPart;
				r.attachedPartId = node.attachedPartId;
				r.breakingForce = node.breakingForce;
				r.breakingTorque = node.breakingTorque;
				r.contactArea = node.contactArea;
				r.icon = node.icon;
				r.nodeType = node.nodeType;
				r.offset = node.offset;
				r.orientation = node.orientation;
				r.originalOrientation = node.originalOrientation;
				r.originalPosition = node.originalPosition;
				r.originalSecondaryAxis = node.originalSecondaryAxis;
				r.overrideDragArea = node.overrideDragArea;
				r.owner = node.owner;
				r.position = node.position;
				r.radius = node.radius;
				r.requestGate = node.requestGate;
				r.secondaryAxis = node.secondaryAxis;
				return r;
			}

			internal static Attachment_t From(AttachNode an)
			{
				Attachment_t r = new Attachment_t();
				r.node = an;
				return r;
			}

			internal AttachNode Info => this.node;

			public bool Equals(AttachNode other) {
				bool r = this.node.id == other.id;

				r &= this.node.nodeTransform.Equals(other.nodeTransform);
				r &= this.node.size == other.size;
				r &= this.node.attachMethod.Equals(other.attachMethod);
				r &= this.node.ResourceXFeed == other.ResourceXFeed;
				r &= this.node.rigid == other.rigid;

				r &= this.node.AllowOneWayXFeed = other.AllowOneWayXFeed;
				r &= this.node.attachedPart.Equals(other.attachedPart);
				r &= this.node.attachedPartId.Equals(other.attachedPartId);
				r &= this.node.breakingForce == other.breakingForce;
				r &= this.node.breakingTorque == other.breakingTorque;
				r &= this.node.contactArea == other.contactArea;
				r &= this.node.icon.Equals(other.icon);
				r &= this.node.nodeType.Equals(other.nodeType);
				r &= this.node.offset.Equals(other.offset);
				r &= this.node.orientation.Equals(other.orientation);
				r &= this.node.originalOrientation.Equals(other.originalOrientation);
				r &= this.node.originalPosition.Equals(other.originalPosition);
				r &= this.node.originalSecondaryAxis.Equals(other.originalSecondaryAxis);
				r &= this.node.overrideDragArea == other.overrideDragArea;
				r &= this.node.owner.Equals(other.owner);
				r &= this.node.position.Equals(other.position);
				r &= this.node.radius == other.radius;
				r &= this.node.requestGate == other.requestGate;
				r &= this.node.secondaryAxis.Equals(other.secondaryAxis);

				return r;
			}
		}

		internal class Attachment_List
		{
			private readonly Dictionary<int, List<Attachment_t>> map = new Dictionary<int, List<Attachment_t>>();
			private readonly object MUTEX = new object();

			public void Clear(Part part)
			{
				lock(MUTEX) this.map[part.GetInstanceID()].Clear();
			}

			public void Destroy(Part part)
			{
				lock(MUTEX) this.map.Remove(part.GetInstanceID());
			}

			public List<Attachment_t> Get(Part part)
			{
				lock(MUTEX) return new List<Attachment_t>(this._list(part));
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
					List<Attachment_t> l = this._list(to);
					l.Clear();
					l.AddRange(this._list(from));
				}
			}

			internal void Update(Part part)
			{
				lock(MUTEX)
				{
					this._list(part).Clear();
					foreach (AttachNode an in part.attachNodes)
						ATTACHMENTS._list(part).Add(Attachment_t.From(an));
				}
			}

			internal void Restore(Part part)
			{
				part.attachNodes.Clear();
				foreach (Attachment_t attachnode in this._list(part))
					part.attachNodes.Add(attachnode.ToPartAttachNode(part));
			}

			private List<Attachment_t> _list(Part part)
			{
				if (!this.map.ContainsKey(part.GetInstanceID())) this.map[part.GetInstanceID()] = new List<Attachment_t>();
				return this.map[part.GetInstanceID()];
			}

		}

		internal static readonly Attachment_List ATTACHMENTS = new Attachment_List();
	}
} }
