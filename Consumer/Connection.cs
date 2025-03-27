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

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Crypto;
using PrivMX.Endpoint.Extra;

namespace Consumer;

public static class Connection
{
	const string password = "password1";
	const string salt = "test";
	const string solutionId = "3b54def9-dca1-434f-9231-f3852a83d878";
	// const string contextId = "8a1bcb91-e34c-4192-a0b7-2bd5975d014c";
	public static async Task<ConnectionSession> CreateSession()
	{
		var coreApp = CryptoApi.Create();
		var privateKey = coreApp.DerivePrivateKey2(password, salt);
		var publicKey = coreApp.DerivePublicKey(privateKey);
		Console.WriteLine($"Private key: {privateKey}");
		Console.WriteLine($"Public key: {publicKey}");
		return await ConnectionSession.Create(privateKey, publicKey, solutionId, "http://localhost:9111");
	}
}