// Copyright (c) Microsoft. All rights reserved.

import 'rxjs';
import { Observable } from 'rxjs';
import moment from 'moment';
import { schema, normalize } from 'normalizr';
import update from 'immutability-helper';
import { createSelector } from 'reselect';
import { redux as appRedux } from './appReducer';
import { IoTHubManagerService } from 'services';
import {
  createReducerScenario,
  createEpicScenario,
  resetPendingAndErrorReducer,
  errorPendingInitialState,
  pendingReducer,
  errorReducer,
  setPending,
  toActionCreator,
  getPending,
  getError
} from 'store/utilities';

// ========================= Epics - START
const handleError = fromAction => error =>
  Observable.of(redux.actions.registerError(fromAction.type, { error, fromAction }));

export const epics = createEpicScenario({
  /** Loads the users */
  fetchUsers: {
    type: 'USERS_FETCH',
    epic: (fromAction, store) => {
      // const conditions = getActiveUserGroupConditions(store.getState());
      return []
      // return IoTHubManagerService.getusers(conditions)
      //   .map(toActionCreator(redux.actions.updateusers, fromAction))
      //   .catch(handleError(fromAction))
    }
  },

  /* Update the users*/
  refreshUsers: {
    type: 'USERS_REFRESH',
    rawEpic: ($actions) =>
      $actions.ofType(appRedux.actionTypes.updateActiveUserGroup)
        .map(({ payload }) => payload)
        .distinctUntilChanged()
        .map(_ => epics.actions.fetchUsers())
  }
});
// ========================= Epics - END

// ========================= Schemas - START
const userSchema = new schema.Entity('users');
const userListSchema = new schema.Array(userSchema);
// ========================= Schemas - END

// ========================= Reducers - START
const initialState = { ...errorPendingInitialState, entities: {}, items: [], lastUpdated: '' };


const deleteUsersReducer = (state, { payload }) => {
  const spliceArr = payload.reduce((idxAcc, payloadItem) => {
    const idx = state.items.indexOf(payloadItem);
    if (idx !== -1) {
      idxAcc.push([idx, 1]);
    }
    return idxAcc;
  }, []);
  return update(state, {
    entities: { $unset: payload },
    items: { $splice: spliceArr }
  });
};

const insertUsersReducer = (state, { payload }) => {
  const inserted = payload.map(user => ({ ...user, isNew: true }));
  const { entities: { users }, result } = normalize(inserted, userListSchema);
  if (state.entities) {
    return update(state, {
      entities: { $merge: users },
      items: { $splice: [[0, 0, ...result]] }
    });
  }
  return update(state, {
    entities: { $set: users },
    items: { $set: result }
  });
};

/* Action types that cause a pending flag */
const fetchableTypes = [
  epics.actionTypes.fetchUsers
];

export const redux = createReducerScenario({
  registerError: { type: 'USERS_REDUCER_ERROR', reducer: errorReducer },
  isFetching: { multiType: fetchableTypes, reducer: pendingReducer },
  deleteUsers: { type: 'USERS_DELETE', reducer: deleteUsersReducer },
  insertUsers: { type: 'USERS_INSERT', reducer: insertUsersReducer },
  resetPendingAndError: { type: 'USERS_REDUCER_RESET_ERROR_PENDING', reducer: resetPendingAndErrorReducer }
});

export const reducer = { users: redux.getReducer(initialState) };
// ========================= Reducers - END

// ========================= Selectors - START
export const getUsersReducer = state => {console.log(state); return state.users};
export const getEntities = state => getUsersReducer(state).entities || {};
export const getItems = state => getUsersReducer(state).items || [];
export const getUsersLastUpdated = state => getUsersReducer(state).lastUpdated;
export const getUsersError = state =>
  getError(getUsersReducer(state), epics.actionTypes.fetchUsers);
export const getUsersPendingStatus = state =>
  getPending(getUsersReducer(state), epics.actionTypes.fetchUsers);
export const getUsers = createSelector(
  getEntities, getItems,
  (entities, items) => items.map(id => entities[id])
);
export const getUserById = (state, id) =>
  getEntities(state)[id];
// ========================= Selectors - END
