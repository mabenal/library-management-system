import { Action, State, StateContext } from "@ngxs/store";
import { ClearUser, SetUser } from "../actions/user.actions";

export interface UserStateModel {
    username: string | null;
    roles: string[] | null;
    userId: string | null;
  }


@State<UserStateModel>({
    name: 'user',
    defaults: {
      username : null,
      roles: [],
      userId: null
    }
  })
export class UserState {
    @Action(SetUser)
    setUser(ctx: StateContext<UserStateModel>, action: SetUser) {
      ctx.patchState({ username: action.username, roles: action.roles, userId: action.userId });
    }

    @Action(ClearUser)
    clearUser(ctx: StateContext<UserStateModel>) {
      ctx.setState({ username: null, roles: null, userId: null });
    }
 }

