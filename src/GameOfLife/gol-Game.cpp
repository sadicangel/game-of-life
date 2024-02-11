module gol;

constexpr char DEAD = '.';
constexpr char ALIVE = '*';

namespace gol
{
    Game::Game(const std::size_t rows, const std::size_t cols)
        :
        _rows(rows),
        _cols(cols),
        _size(rows * cols),
        _main(new char[rows * cols]),
        _temp(new char[rows * cols]),
        _error("")
    {
        std::memset(_main.get(), DEAD, _size);
        std::memset(_temp.get(), DEAD, _size);
    }

    void Game::error(char* out, const std::size_t size) const
    {
        std::memcpy(out, _error.data(), std::min(size, _error.size()));
    }

    std::size_t Game::seed(const char* seed, const std::size_t size)
    {
        if (size != _size) {
            _error = "Expected seed of size to be '" + std::to_string(_size) + "'. Got '" + std::to_string(size) + "'";
            return _error.size();
        }

        std::memcpy(_main.get(), seed, size);

        return 0;
    }

    const char* Game::state() const
    {
        return _main.get();
    }

    const char Game::get_next_state(std::size_t i) const
    {
        const auto ul = i - _cols - 1;
        const auto up = i - _cols;
        const auto ur = i - _cols + 1;
        const auto le = i - 1;
        const auto ri = i + 1;
        const auto dl = i + _cols - 1;
        const auto dp = i + _cols;
        const auto dr = i + _cols + 1;

        size_t living = 0;
        const auto main = _main.get();
        if (ul >= 0 && ul < _size && main[ul] == ALIVE)
            living++;
        if (up >= 0 && up < _size && main[up] == ALIVE)
            living++;
        if (ur >= 0 && ur < _size && main[ur] == ALIVE)
            living++;
        if (le >= 0 && le < _size && main[le] == ALIVE)
            living++;
        if (ri >= 0 && ri < _size && main[ri] == ALIVE)
            living++;
        if (dl >= 0 && dl < _size && main[dl] == ALIVE)
            living++;
        if (dp >= 0 && dp < _size && main[dp] == ALIVE)
            living++;
        if (dr >= 0 && dr < _size && main[dr] == ALIVE)
            living++;

        const auto cell = main[i];
        if (cell == ALIVE) {
            return living == 2 || living == 3 ? ALIVE : DEAD;
        }
        else {
            return living == 3 ? ALIVE : DEAD;
        }
    }

    void Game::tick()
    {
        // TODO: Make this run in parallel?
        auto temp = _temp.get();
        for (size_t y = 0; y < _rows; ++y) {
            for (size_t x = 0; x < _cols; ++x) {
                const auto i = y * _cols + x;
                temp[i] = get_next_state(i);
            }
        }
        std::swap(_main, _temp);
        _generation++;
    }

    std::size_t Game::generation() const
    {
        return _generation;
    }
}