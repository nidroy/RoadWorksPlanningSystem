import json
import os
import sys

import numpy as np
import pandas as pd

from io import StringIO

from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import PolynomialFeatures
from sklearn.tree import DecisionTreeRegressor
from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor
from sklearn.svm import SVR
from sklearn.neighbors import KNeighborsRegressor
from xgboost import XGBRegressor
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, LSTM, SimpleRNN, GRU


# Устанавливаем кодировку в UTF-8
sys.stdout.reconfigure(encoding='utf-8')
sys.stderr.reconfigure(encoding='utf-8')


# Методы прогнозирования
def linear_regression_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = LinearRegression()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def polynomial_regression_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    poly = PolynomialFeatures(degree=2)
    x_poly = poly.fit_transform(x_train)
    model = LinearRegression()
    model.fit(x_poly, y_train, verbose=0)
    x_pred_poly = poly.transform(x_pred)
    y_pred = model.predict(x_pred_poly)[0]
    return y_pred


def decision_tree_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = DecisionTreeRegressor()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def random_forest_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = RandomForestRegressor()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def svr_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = SVR()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def neural_network_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = Sequential([
        Dense(10, activation='relu', input_shape=(1,)),
        Dense(1)
    ])
    model.compile(optimizer='adam', loss='mse')
    model.fit(x_train, y_train, epochs=100, batch_size=10, verbose=0)
    y_pred = model.predict(x_pred)[0, 0]
    return y_pred


def lstm_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = Sequential()
    model.add(LSTM(4, input_shape=(1, 1)))
    model.add(Dense(1))
    model.compile(loss='mean_squared_error', optimizer='adam')
    model.fit(x_train, y_train, epochs=100, batch_size=10, verbose=0)
    y_pred = model.predict(x_pred)[0, 0]
    return y_pred


def simple_rnn_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = Sequential([
        SimpleRNN(10, activation='relu', input_shape=(1, 1)),
        Dense(1)
    ])
    model.compile(optimizer='adam', loss='mse')
    model.fit(x_train, y_train, epochs=100, batch_size=10, verbose=0)
    y_pred = model.predict(x_pred)[0, 0]
    return y_pred


def gru_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = Sequential([
        GRU(10, activation='relu', input_shape=(1, 1)),
        Dense(1)
    ])
    model.compile(optimizer='adam', loss='mse')
    model.fit(x_train, y_train, epochs=100, batch_size=10, verbose=0)
    y_pred = model.predict(x_pred)[0, 0]
    return y_pred


def knn_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = KNeighborsRegressor(n_neighbors=5)
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def gradient_boosting_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = GradientBoostingRegressor()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def xgboost_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = XGBRegressor()
    model.fit(x_train, y_train, verbose=0)
    y_pred = model.predict(x_pred)[0]
    return y_pred


# Загрузка данных из файла
current_directory = os.path.dirname(os.path.abspath(__file__))
data_path = os.path.join(current_directory, 'data.json')

with open(data_path, 'r', encoding='utf-8') as file:
    json_data = file.read()

df = pd.read_json(StringIO(json_data))
months = [
    'Январь',
    'Февраль',
    'Март',
    'Апрель',
    'Май',
    'Июнь',
    'Июль',
    'Август',
    'Сентябрь',
    'Октябрь',
    'Ноябрь',
    'Декабрь'
]
table = df.pivot_table(values='TechnicalCondition', index='RoadId', columns=['Year', 'Month'], aggfunc='first').reindex(columns=pd.MultiIndex.from_product([df['Year'].unique(), months]))
table.columns = range(0, len(table.columns))

table = table.dropna(axis=1)

predictions = {}

for index, row in table.iterrows():
    values = row.values.tolist()
    key = index
    prediction = polynomial_regression_predict(values)
    predictions[key] = prediction

predictions_json = json.dumps(predictions)
print(predictions_json)
