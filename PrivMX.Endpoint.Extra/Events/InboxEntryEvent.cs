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
using PrivMX.Endpoint.Inbox.Models;

namespace PrivMX.Endpoint.Extra.Events;

public partial struct InboxEntryEvent : IUnion<InboxEntryCreatedEvent, InboxEntryDeletedEvent>;