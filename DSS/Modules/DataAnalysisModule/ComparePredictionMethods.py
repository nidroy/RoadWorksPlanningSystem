import sys

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

from io import StringIO

from sklearn.metrics import mean_squared_error, mean_absolute_error

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
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def polynomial_regression_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    poly = PolynomialFeatures(degree=2)
    x_poly = poly.fit_transform(x_train)
    model = LinearRegression()
    model.fit(x_poly, y_train)
    x_pred_poly = poly.transform(x_pred)
    y_pred = model.predict(x_pred_poly)[0]
    return y_pred


def decision_tree_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = DecisionTreeRegressor()
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def random_forest_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = RandomForestRegressor()
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def svr_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = SVR()
    model.fit(x_train, y_train)
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
    model.fit(x_train, y_train, epochs=100, batch_size=10)
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
    model.fit(x_train, y_train, epochs=100, batch_size=10)
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
    model.fit(x_train, y_train, epochs=100, batch_size=10)
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
    model.fit(x_train, y_train, epochs=100, batch_size=10)
    y_pred = model.predict(x_pred)[0, 0]
    return y_pred


def knn_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = KNeighborsRegressor(n_neighbors=5)
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def gradient_boosting_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = GradientBoostingRegressor()
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


def xgboost_predict(data):
    x_train = np.arange(len(data)-1).reshape(-1, 1)
    y_train = data[:-1]
    x_pred = np.array(len(data)-1).reshape(-1, 1)
    model = XGBRegressor()
    model.fit(x_train, y_train)
    y_pred = model.predict(x_pred)[0]
    return y_pred


# Загрузка данных из файла
with open('data.json', 'r', encoding='utf-8') as file:
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

y = np.array(table.iloc[2])
x = np.arange(len(y))
y_test = y[-24:]
y = y[:-24]

predictions = {
        'Linear Regression': [],
        'Polynomial Regression': [],
        'Decision Tree': [],
        'Random Forest': [],
        'SVR': [],
        'Neural Network': [],
        'LSTM': [],
        'Simple RNN': [],
        'GRU': [],
        'KNN': [],
        'Gradient Boosting': [],
        'XGBoost': []
    }

features = {
        'Linear Regression': y.copy(),
        'Polynomial Regression': y.copy(),
        'Decision Tree': y.copy(),
        'Random Forest': y.copy(),
        'SVR': y.copy(),
        'Neural Network': y.copy(),
        'LSTM': y.copy(),
        'Simple RNN': y.copy(),
        'GRU': y.copy(),
        'KNN': y.copy(),
        'Gradient Boosting': y.copy(),
        'XGBoost': y.copy()
    }

# Linear Regression
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = linear_regression_predict(features['Linear Regression'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Linear Regression'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Linear Regression'] = np.append(features['Linear Regression'], predicted_value)
print('Linear Regression complete')

# Polynomial Regression
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = polynomial_regression_predict(features['Polynomial Regression'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Polynomial Regression'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Polynomial Regression'] = np.append(features['Polynomial Regression'], predicted_value)
print('Polynomial Regression complete')

# Decision Tree
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = decision_tree_predict(features['Decision Tree'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Decision Tree'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Decision Tree'] = np.append(features['Decision Tree'], predicted_value)
print('Decision Tree complete')

# Random Forest
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = random_forest_predict(features['Random Forest'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Random Forest'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Random Forest'] = np.append(features['Random Forest'], predicted_value)
print('Random Forest complete')

# SVR
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = svr_predict(features['SVR'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['SVR'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['SVR'] = np.append(features['SVR'], predicted_value)
print('SVR complete')

# Neural Network
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = neural_network_predict(features['Neural Network'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Neural Network'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Neural Network'] = np.append(features['Neural Network'], predicted_value)
print('Neural Network complete')

# LSTM
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = lstm_predict(features['LSTM'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['LSTM'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['LSTM'] = np.append(features['LSTM'], predicted_value)
print('LSTM complete')

# Simple RNN
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = simple_rnn_predict(features['Simple RNN'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Simple RNN'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Simple RNN'] = np.append(features['Simple RNN'], predicted_value)
print('Simple RNN complete')

# GRU
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = gru_predict(features['GRU'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['GRU'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['GRU'] = np.append(features['GRU'], predicted_value)
print('GRU complete')

# KNN
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = knn_predict(features['KNN'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['KNN'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['KNN'] = np.append(features['KNN'], predicted_value)
print('KNN complete')

# Gradient Boosting
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = gradient_boosting_predict(features['Gradient Boosting'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['Gradient Boosting'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['Gradient Boosting'] = np.append(features['Gradient Boosting'], predicted_value)
print('Gradient Boosting complete')

# XGBoost
for _ in range(len(y_test)):
    # Предсказываем одну точку
    predicted_value = xgboost_predict(features['XGBoost'])
    # добавляем предсказанное значение в массив для текущего метода
    predictions['XGBoost'].append(predicted_value)
    # Обновляем массив features для текущего метода
    features['XGBoost'] = np.append(features['XGBoost'], predicted_value)
print('XGBoost complete')

# Метрики оценки
for method, values in predictions.items():
    mse = mean_squared_error(list(y_test), values)
    mae = mean_absolute_error(list(y_test), values)
    print(method)
    print(f'MSE: {mse}')
    print(f'MAE: {mae}')
    print('')

# Визуализация результатов
plt.scatter(range(len(y)), y, label='Исходные данные')
plt.scatter(range(len(y), len(y) + len(y_test)), y_test, label='Тестовые данные')

for method, values in predictions.items():
    plt.plot(range(len(y), len(y) + len(values)), values, label=method)

plt.legend()
plt.show()
