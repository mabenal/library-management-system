import { Action, State, StateContext } from "@ngxs/store";
import { ClearUser, SetUser } from "../actions/user.actions";

export interface UserStateModel {
    username: string | null;
    roles: string[] | null;
  }


@State<UserStateModel>({
    name: 'user',
    defaults: {
      username : null,
      roles: []
    }
  })
export class UserState {
    @Action(SetUser)
    setUser(ctx: StateContext<UserStateModel>, action: SetUser) {
      ctx.patchState({ username: action.username, roles: action.roles });
    }

    @Action(ClearUser)
    clearUser(ctx: StateContext<UserStateModel>) {
      ctx.setState({ username: null, roles: null });
    }
 }

