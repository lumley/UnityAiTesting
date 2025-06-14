using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// Controller for managing the timeline of events in the game.
    /// </summary>
    public sealed class TimelineController : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector _playableDirector = null!;

        public async Task PlayTimelineAsync()
        {
            await PlayAndWaitForTimeline(_playableDirector);
        }

        private Task PlayAndWaitForTimeline(PlayableDirector director)
        {
            var tcs = new TaskCompletionSource<bool>();

            director.stopped += OnStopped;
            director.Play();

            return tcs.Task;

            void OnStopped(PlayableDirector pd)
            {
                director.stopped -= OnStopped;
                tcs.SetResult(true);
            }
        }
    }
}