import { applyMiddleware, createStore } from "redux";
import { configureStore } from "@reduxjs/toolkit";
import { composeWithDevTools } from "redux-devtools-extension";
import thunk from "redux-thunk";

import { persistStore, persistReducer } from "redux-persist";
import storage from "redux-persist/lib/storage";

import reducers from "./reducers/redcuers";

const middlware = [thunk];

// const store = createStore(
//   reducers,
//   composeWithDevTools(applyMiddleware(...middlware))
// );
const persistConfig = {
  key: "main-root",
  storage,
};
const persistedReducer = persistReducer(persistConfig, reducers);

const store = configureStore({
  reducer: reducers,
  middleware: [thunk],
});
const Persistor = persistStore(store);

export { Persistor };

export default store;

// const bindMiddlware = (middlware) => {
//   if (process.env.NODE_ENV !== "production") {
//     const { composeWithDevTools } = require("redux-devtools-extension");
//     return composeWithDevTools(applyMiddleware(...middlware));
//   }

//   return applyMiddleware(...middlware);
// };

// const reducer = (state, action) => {
//   if (action.type === HYDRATE) {
//     const nextState = {
//       ...state,
//       ...action.payload,
//     };
//     return nextState;
//   } else {
//     return reducers(state, action);
//   }
// };

// const initStore = () => {
//   return createStore(reducer, bindMiddlware([thunkMiddleware]));
// };

// export const wrapper = createWrapper(initStore);
