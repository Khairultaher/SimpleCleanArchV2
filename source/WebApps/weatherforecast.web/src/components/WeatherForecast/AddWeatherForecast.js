import React, { Component, Fragment, useState, useEffect, useRef } from "react";
import Pagination from "react-js-pagination";
import { Modal } from "bootstrap";

import { useDispatch, useSelector } from "react-redux";
import { useAlert } from "react-alert";

import {
  addForcast,
  editForcast,
  clearErrors,
} from "../../redux/actions/weatherForcastActions";

import {
  ADD_FORCAST_RESET,
  CLEAR_ERRORS,
} from "../../redux/constants/weatherForcastConstants";

import Loader from "../layout/Loader";

const AddWeatherForecast = (props) => {
  const [temperatureC, setTemperatureC] = useState(0);
  const [location, setLocation] = useState("");
  const [summary, setSummary] = useState("");

  const alert = useAlert();
  const modalRef = useRef();
  const dispatch = useDispatch();
  const { loading, error, success, id } = useSelector(
    (state) => state.addForcast
  );

  useEffect(() => {
    if (props.show) {
      setTemperatureC(props.data.temperatureC);
      setLocation(props.data.location);
      setSummary(props.data.summary);
      showModal();
    }
  }, [props.show]);

  const showModal = () => {
    const modalEle = modalRef.current;
    const bsModal = new Modal(modalEle, {
      backdrop: "static",
      keyboard: false,
    });
    bsModal.show();
  };

  const hideModal = () => {
    const modalEle = modalRef.current;
    const bsModal = Modal.getInstance(modalEle);

    setTemperatureC(0);
    setSummary(null);
    setLocation(null);

    bsModal.hide();
    props.onCloseModalClick();
  };

  const submitHandler = async () => {
    const formData = new FormData();
    formData.set("id", props.data.id);
    formData.set("temperatureC", temperatureC);
    formData.set("summary", summary);
    formData.set("location", location);

    if (!props.data.id) {
      await dispatch(addForcast(formData));
    } else {
      await dispatch(editForcast(formData));
    }

    if (success) {
      hideModal();
    }
  };

  return (
    <div className="modal" ref={modalRef} tabIndex="-1">
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id="staticBackdropLabel">
              {!props.data.id ? "Add" : "Edit"} Weather Forecast
            </h5>
            <button
              type="button"
              className="close"
              onClick={hideModal}
              data-dismiss="modal"
              aria-label="Close"
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="modal-body">
            <form className="">
              <div className="form-group">
                <label htmlFor="location" className="col-form-label">
                  Location:
                </label>
                <input
                  className="form-control"
                  id="location"
                  value={location}
                  onChange={(e) => setLocation(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label htmlFor="tempc" className="col-form-label">
                  Temp. (C):
                </label>
                <input
                  type="number"
                  className="form-control"
                  id="tempc"
                  value={temperatureC}
                  onChange={(e) => setTemperatureC(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label htmlFor="summary" className="col-form-label">
                  Summary:
                </label>
                <textarea
                  className="form-control"
                  id="summary"
                  value={summary}
                  onChange={(e) => setSummary(e.target.value)}
                />
              </div>
            </form>
          </div>
          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={hideModal}
              data-dismiss="modal"
              disabled={loading}
            >
              Close
            </button>
            <button
              type="button"
              className="btn btn-primary"
              disabled={loading}
              onClick={submitHandler}
              data-dismiss="modal"
              aria-label="Close"
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AddWeatherForecast;
