from mlagents_envs.base_env import ActionTuple
import numpy as np
from ddpg_agent import Agent
from mlagents_envs.environment import UnityEnvironment

n_episodes = 1000

best_score = -1000
score_history = []
load_checkpoint = False

print("Open in Unity")
env = UnityEnvironment(file_name=None, seed=1, side_channels=[])
# Start interacting with the environment.
env.reset()
behavior_name = list(env.behavior_specs)[0]
spec = env.behavior_specs[behavior_name]


decision_steps, terminal_steps = env.get_steps(behavior_name)
observation_size = (decision_steps[0][0][0]).shape
#action_size = len(decision_steps.action_mask)
action_size = 2
print("Actions: ", action_size)
print("Observation: ", observation_size)


agent = Agent(input_dims=observation_size, n_actions=action_size)
i = 0

while i < 2500000:
    env.reset()
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    observation = decision_steps[0][0][0]
    done = False
    score = 0
    while not done:
        i += 1
        tracked_agent = decision_steps.agent_id[0]
        
        action = agent.act(observation, False)
        #print(observation)
        #print(action)
        action_tuo = ActionTuple(continuous=action)
        env.set_actions(behavior_name, action_tuo)
        env.step()
        decision_steps, terminal_steps = env.get_steps(behavior_name)
        
        if tracked_agent in decision_steps:
            observation_ = decision_steps[0][0][0]
            reward = decision_steps[tracked_agent].reward
        if tracked_agent in terminal_steps:
            observation_ = terminal_steps[0][0][0]
            reward = terminal_steps[tracked_agent].reward
            done = True
        
        score += reward
        agent.remember(observation, action, reward, observation_, done)
        agent.learn()
        observation = observation_
        
    score_history.append(score)
    avg_score = np.mean(score_history[-100:])

    if avg_score > best_score:
        best_score = avg_score
        if not load_checkpoint:
            agent.save_models()

    print('step ', i, 'score %.1f' % score, 'avg score %.1f' % avg_score)
    