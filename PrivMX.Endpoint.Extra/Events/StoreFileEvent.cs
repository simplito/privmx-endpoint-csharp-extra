//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Internal.Unions;
using PrivMX.Endpoint.Store.Models;
using PrivMX.Endpoint.Thread.Models;

namespace PrivMX.Endpoint.Extra.Events;

/// <summary>
///     Union of multiple store file events.
/// </summary>
public partial struct StoreFileEvent : IUnion<StoreFileCreatedEvent, StoreFileUpdatedEvent, StoreFileDeletedEvent>;