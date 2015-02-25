// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System.Collections;
using PF.Base;
using UnityEngine.UI;

/*
 * Handles dialog box shown in game
 * Spins a gacha if npcAsset is porvided at end of dialog
 */
using System;


public class StartGame : MonoBehaviour
{
    void Start()
    {
        AppController.Instance.Init();
    }
}