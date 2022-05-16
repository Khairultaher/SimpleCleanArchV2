import React, { Component, Fragment, useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useAlert } from "react-alert";
import { useNavigate, useLocation } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import Loader from "../layout/Loader";

import { logout, clearErrors } from "../../redux/actions/authActions";

const Logout = ({ props }) => {
  const { setAuth } = useAuth();
  const [userName, setUserName] = useState();
  const [password, setPassword] = useState();

  const alert = useAlert();
  const dispatch = useDispatch();
  const { loading, isAuthenticated, user, error } = useSelector(
    (state) => state.auth
  );

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  useEffect(() => {
    console.log("logout");
    // if (error) {
    //   alert.error(error);
    //   dispatch(clearErrors());
    // }
    if (isAuthenticated) {
      dispatch(logout());
    } else {
      setAuth({});
      navigate(from, { replace: true });
    }
  }, [dispatch, isAuthenticated]);

  return <Fragment>{loading ? <Loader /> : <Fragment></Fragment>}</Fragment>;
};

export default Logout;
